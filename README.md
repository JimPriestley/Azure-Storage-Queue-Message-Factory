d# Introduction 
This a sample class library for using the Azure Storage Queue service. It includes methods for sending and receiving strings and objects as queue messages.

# Getting Started
1.	This sample is built in .NET v 4.62 in Visual Studio 2017, but can be backported without change to older versions of .NET and VS that are supported by the Microsoft WindowsAzure.Storage nuget package.
2.	This sample was built and tested with WindowsAzure.Storage nuget package version 9.10

# Azure Storage Queues
Azure Storage Queues are different from Service Bus, in that the require the messages to be idempotent.
A message that says the new value is 7 is a valid idempotent message. A message that says add 7 to the current total, is not a valid idempotent message.

Azure Service Bus delivers messages First In, First Out, and every message is only delivered once.

Azure Storage Queues make a best effort at order, and if you have multiple listeners in a distrubuted architecture, it is possible that more than one listener may receive a message.

Based on the lower cost of Storage Queues versus Service Bus or Event Hubs, I often recommend it in distributed systems architectures moving to Azure PaaS, as long as we can verify that the messages meet the idempotent requirement.

# Contributing
I welcome feedback and contributions on this sample.

Opportunities for extending this class include:
- Wrapping the Async methods for Storage Queues
- Exposing the Batch Read Methods for Storage Queues

# License

MIT License

Copyright (c) 2018 Jim Priestley

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
