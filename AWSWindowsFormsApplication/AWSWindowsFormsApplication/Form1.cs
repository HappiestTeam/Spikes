using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Amazon.SQS;
using Amazon;
using Amazon.SQS.Model;

namespace AWSWindowsFormsApplication
{
    public partial class Form1 : Form
    {
        AmazonSQS sqs = AWSClientFactory.CreateAmazonSQSClient(RegionEndpoint.USWest2);
        String myQueueUrl;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateQueueRequest sqsRequest = new CreateQueueRequest();
            sqsRequest.QueueName = "MYFirstQueue";
            CreateQueueResponse createQueueResponse = sqs.CreateQueue(sqsRequest);            
            myQueueUrl = createQueueResponse.CreateQueueResult.QueueUrl;

            //Confirming the queue exists
            ListQueuesRequest listQueuesRequest = new ListQueuesRequest();
            ListQueuesResponse listQueuesResponse = sqs.ListQueues(listQueuesRequest);
            
            SendMessageRequest sendMessageRequest = new SendMessageRequest();
            sendMessageRequest.QueueUrl = myQueueUrl; 
            sendMessageRequest.MessageBody = txtPushMsg.Text;
            sqs.SendMessage(sendMessageRequest);
        

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest();
            receiveMessageRequest.QueueUrl = myQueueUrl;
            ReceiveMessageResponse receiveMessageResponse = sqs.ReceiveMessage(receiveMessageRequest);
            if (receiveMessageResponse.IsSetReceiveMessageResult())
            {
                ReceiveMessageResult receiveMessageResult = receiveMessageResponse.ReceiveMessageResult;
                foreach (Amazon.SQS.Model.Message message in receiveMessageResult.Message)
                {
                    if (message.IsSetBody())
                     MessageBox.Show(message.Body);
                }
            }

            String messageRecieptHandle = receiveMessageResponse.ReceiveMessageResult.Message[0].ReceiptHandle;
            DeleteMessageRequest deleteRequest = new DeleteMessageRequest();
            deleteRequest.QueueUrl = myQueueUrl;
            deleteRequest.ReceiptHandle = messageRecieptHandle;
            sqs.DeleteMessage(deleteRequest);
        }
    }
}
