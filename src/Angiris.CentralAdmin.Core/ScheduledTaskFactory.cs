﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Angiris.Core.Models;
using Angiris.Core.Messaging;
using Angiris.Core.DataStore;

namespace Angiris.CentralAdmin.Core
{
    public class ScheduledFlightCrawlRequestFactory
    {
        IQueueTopicManager<FlightCrawlEntity> queueManagerP0;
        IQueueTopicManager<FlightCrawlEntity> queueManager;
        INoSqlStoreProvider<FlightCrawlEntity> cacheStore;
        //INoSQLStoreProvider<FlightCrawlEntity> persistenceStore;

        public void Initialize()
        {
            
        }
 
        public async Task StartPushTaskMessages(int totalMessages, Action<string> onActionUpdate)
        {
            var qMgrProfile= QueueMgrProfile.Default;
            qMgrProfile.IsHighPriority = false;

            queueManager = QueueManagerFactory.CreateFlightCrawlEntityQueueMgr(qMgrProfile);
            queueManager.Initialize();

            var qMgrProfileP0 = QueueMgrProfile.Default;
            qMgrProfileP0.IsHighPriority = true;

            queueManagerP0 = QueueManagerFactory.CreateFlightCrawlEntityQueueMgr(qMgrProfileP0);
            queueManagerP0.Initialize();

            cacheStore = DataProviderFactory.SingletonRedisQueuedTaskStore;
            

            //persistenceStore = DataProviderFactory.GetDocDBQueuedTaskStore<FlightCrawlEntity>();
            //persistenceStore.Initialize();




            var crawlRequests = FakeDataRepo.GenerateRandomFlightCrawlRequests(Convert.ToInt32(totalMessages*0.8));
            var crawlRequestsP0 = FakeDataRepo.GenerateRandomFlightCrawlRequests(Convert.ToInt32(totalMessages * 0.2));
           //int i = 0;

            var allrequests = crawlRequests.Concat(crawlRequestsP0).OrderBy(r => Guid.NewGuid()).ToList();

            //ParallelOptions pOption = new ParallelOptions();
            //pOption.MaxDegreeOfParallelism = 20;

            var startTime = DateTime.Now;

            List<Task> displayResultTaskList = new List<Task>();

            var pRequestSendingResult = Parallel.ForEach(allrequests, (r) =>  {
            //crawlRequests.ForEach((r) =>  {

                Task.Run(async () =>
                {
                    //Console.WriteLine("processing " + i.ToString());
                    var currentTime = DateTime.UtcNow;
                    r.CreateTime = currentTime;
                    r.LastModifiedTime = currentTime;
                    r.Status = Angiris.Core.Models.TaskStatus.New;
                    r.TaskId = r.RequestData.DistinctHash;


                    await cacheStore.CreateEntity(r);
                    bool sendResult;
                    if(crawlRequestsP0.Contains(r))
                    {
                        sendResult = await queueManagerP0.SendMessage(r);
                    }
                    else
                    {
                        sendResult = await queueManager.SendMessage(r);
                    }

                    if (sendResult)
                    {
                        r.Status = Angiris.Core.Models.TaskStatus.Queueing;
                        r.LastModifiedTime = DateTime.UtcNow;
                        await cacheStore.UpdateEntity(r.TaskId, r);

                        onActionUpdate("done sending out task message " + r.TaskId);
                        //onActionUpdate()
                    }
                    //await persistenceStore.CreateEntity(r);

                    var displayResultTask = Task.Run(async () =>
                    {
                        string taskID = r.TaskId;


                        var getStatusTask = GetQueuedStatusAsync(taskID, 120, 1000);
                        var msGetStatusTimeout = 120000; //2 min
                        var getStatusTaskStart = DateTime.UtcNow;
                        if (await Task.WhenAny(getStatusTask, Task.Delay(msGetStatusTimeout)) == getStatusTask)
                        {
                            var taskExecutionLength = DateTime.UtcNow - getStatusTaskStart;
                            var getStatusTaskResult = getStatusTask.Result;

                            onActionUpdate("Task " + getStatusTaskResult.TaskId + " completed in "
                                + taskExecutionLength.TotalSeconds.ToString("F") + "");
                        }
                        else
                        {
                            onActionUpdate("Task " + taskID + " reached max timeout of " + msGetStatusTimeout.ToString() + "ms. ");
                        }
                    });

                    displayResultTaskList.Add(displayResultTask);

                    //Console.WriteLine(i++);
                }).Wait();
            });

            Task.WaitAll(displayResultTaskList.ToArray());

            var endTime = DateTime.Now;
            onActionUpdate("End in " + (endTime - startTime).TotalSeconds + " seconds");
        }

   

        private async Task<IQueuedTask> GetQueuedStatusAsync(string taskID, int maxAttempts, int waitPeriodInMilliSeconds)
        {
            int attempts = 1;
            IQueuedTask output = null;

            while(attempts <= maxAttempts)
            {
                try
                {                

                    var result = await cacheStore.ReadEntity(taskID);

                    if (result != null && result.Status == Angiris.Core.Models.TaskStatus.Completed)
                    {
                        output = result;
                        break;
                    }
                    else
                        await Task.Delay(TimeSpan.FromMilliseconds(waitPeriodInMilliSeconds));

                }
                catch(Exception ex)
                {

                }
                finally
                {
                    attempts++;
                }                
            }

            return output;


        }
 

    }
}
