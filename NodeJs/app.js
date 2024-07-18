const express = require('express');
const { PubSub } = require('@google-cloud/pubsub');

const app = express();
const port = 3000;
const projectId = 'dell-demo-project-03'
const topicName = 'pubsub-test';
// 創建 Pub/Sub 客戶端
// 確保您的環境中已設置好 Google Cloud 憑證
const pubsub = new PubSub({ projectId: projectId });

// 替換為您的 Pub/Sub topic 名稱

async function publishMessage(data) {
    const dataBuffer = Buffer.from(JSON.stringify(data));
    try {
        const messageId = await pubsub.topic(topicName).publish(dataBuffer);
        return messageId;
    } catch (error) {
        console.error('Error publishing message:', error);
        throw error;
    }
}

app.get('/benchmark', async (req, res) => {
    const totalMessages = 10000;
    const batchSize = 1500; // 每批發送的消息數量

    console.log(`Starting benchmark: Publishing ${totalMessages} messages to ${topicName}`);

    const startTime = process.hrtime.bigint();

    const batches = Array(Math.ceil(totalMessages / batchSize)).fill().map((_, i) => {
        const start = i * batchSize;
        const end = Math.min(start + batchSize, totalMessages);
        return Array(end - start).fill().map((_, j) => ({
            id: start + j + 1,
            timestamp: new Date().toISOString()
        }));
    });

    try {
        for (const batch of batches) {
            await Promise.all(batch.map(message => publishMessage(message)));
        }

        const endTime = process.hrtime.bigint();
        const totalTimeNs = endTime - startTime;
        const totalTimeMs = Number(totalTimeNs) / 1_000_000;
        const totalTimeSec = totalTimeMs / 1000;

        const averageTimeMs = totalTimeMs / totalMessages;
        const messagesPerSecond = totalMessages / totalTimeSec;

        const result = {
            totalMessages,
            totalTimeSeconds: totalTimeSec,
            averageTimeMs: averageTimeMs,
            messagesPerSecond: messagesPerSecond
        };

        console.log('Benchmark result:', result);
        res.json(result);
    } catch (error) {
        console.error('Error during benchmark:', error);
        res.status(500).json({ error: 'Benchmark failed' });
    }
});

app.listen(port, () => {
    console.log(`Server running at http://localhost:${port}`);
});