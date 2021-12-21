'use strict';

/**
 * Streaming speech-to-text demo program.
 */

const fs = require('fs');
const WebSocket = require('ws');
const envConfigs = require('./account');
const fetch = require('node-fetch');
const { commandType, responseType } = require('./const');

const env = envConfigs.signansStg;
const apiPath = '/api/v1/translate/stt-streaming';
const tokenPath = '/api/v1/token';
const speechData = {
    language: 'ja',
    samplingRate: 16000,
    audioFile: 'ja_jp.wav'
};

const connections = new Map();
const start = Date.now();
const TEST_TARGET = 30;
let stopAdd = false;
let completeCount = 0;

let nextTimer = null;

const getJwtToken = async (url, accessKey, secretKey) => {
    const data = {
        accessKey,
        secretKey,
        duration: 300
    };
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    });
    const responseJSON = await response.json();
    return responseJSON.data.encodedJWT;
}

const handleSessionMessage = (connection, key, message) => {
    const messageJSON = JSON.parse(message);
    switch (messageJSON.type) {
        case responseType.languageReady:
            // The language is set. Set the sampling rate.
            console.log('Language is set. Set sampling rate.');
            connection.send(JSON.stringify({
                command: commandType.setSamplingRate,
                value: speechData.samplingRate,
            }));
            break;
        case responseType.languageError:
        case responseType.samplingRateError:
            console.error(messageJSON.value);
            break;
        case responseType.samplingRateReady:
            // The language is set. Send the audio data stream.
            console.log('Sampling rate is set. Send audio data stream.');
            fs.createReadStream(speechData.audioFile).on('data', (buf) => {
                connection.send(buf, (error) => {
                    if (error) {
                        console.error(error.message);
                    }
                });
            }).on('end', () => connection.send(JSON.stringify({
                command: commandType.endStream,
            })));
            break;
        case responseType.recognitionResult:
            if (messageJSON.status === 'recognized') {
                console.log(`${key}: ${new Date().toLocaleTimeString('ja-JP')}, ${messageJSON.value}`);
            }
            break;
        case responseType.recognitionError:
            console.error('Recognition error:');
            console.error(messageJSON.value);
            // In case of error, we close the connection immediately.
            connection.send(JSON.stringify({
                command: commandType.endSession,
            }));
            break;
        default:
            console.log('Unexpected response type:');
            console.log(messageJSON.type);
    }
};

const newConnection = async (tokenUrl, accessKey, secretKey) => {
    const token = await getJwtToken(tokenUrl, accessKey, secretKey);
    if (token) {
        const url = `${env.host}${apiPath}?token=Bearer ${token}`;
        const connection = new WebSocket(url);
        const key = Date.now();
        connections.set(key, connection);
        console.log(`Connection count: ${connections.size}`);
        connection.on('open', () => {
            console.log('Connected to streaming STT API.');
            // Once connected, set the speech language.
            if (speechData.language) {
                connection.send(JSON.stringify({
                    command: commandType.setLanguage,
                    value: speechData.language,
                }));
            } else {
                connection.send(JSON.stringify({
                    command: commandType.setSamplingRate,
                    value: speechData.samplingRate,
                }));
            }
        });
        connection.on('message', (message) => {
            handleSessionMessage(connection, key, message);
        });
        connection.on('error', (code, reason) => {
            console.error(`Error: ${code}, ${reason}`);
        });
        connection.on('close', (code, reason) => {
            console.log(`Connection closed: ${code}, ${reason}`);
            connections.delete(key);
            completeCount++;
            console.log(`Connection count: ${connections.size}`);
        });
    }

    
    if (connections.size < TEST_TARGET && !stopAdd) {
        clearTimeout(nextTimer);
        nextTimer = setTimeout(async () => {
            await newConnection(tokenUrl, accessKey, secretKey);
        }, Math.random() * 1000);
    }
    if (connections.size === TEST_TARGET) {
        stopAdd = true;
    }
    
}

const main = async () => {
    const { accessKey, secretKey } = env.authConfig;
    const tokenUrl = `${env.host.replace('ws', 'http')}${tokenPath}`;
    await newConnection(tokenUrl, accessKey, secretKey);
};

main();
