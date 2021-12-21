'use strict';

/**
 * Streaming speech-to-text demo program.
 */

const fs = require('fs');
const WebSocket = require('ws');
const envConfigs = require('./account');
const fetch = require('node-fetch');
const { commandType, responseType } = require('./const');

const apiPath = '/api/v1/translate/stt-streaming';
const tokenPath = '/api/v1/token';
const speechData = {
    language: 'ja',
    samplingRate: 16000,
    audioFile: 'ja_jp.wav',
    //phraseList: ['星系文明', '3级文明']
    //phraseList: []
};

let pre = Date.now();


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

const handleSessionMessage = (connection, message) => {
    const messageJSON = JSON.parse(message);
    switch (messageJSON.type) {
        case responseType.languageReady:
            // The language is set. Set the sampling rate.
            console.log('Language is set. Setting sampling rate.');
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
            console.log('Sample rate send suucess');
            if (speechData.phraseList) {
                console.log('Set phrase list', speechData.phraseList);
                connection.send(JSON.stringify({
                    command: commandType.setPhraseList,
                    value: speechData.phraseList,
                }));

            } else {
                console.log('Sending audio data...');
                readAndSendAudioFile(connection);
            }
            break;
        case responseType.phraseListReady:
        case responseType.phraseListError:
            console.log('Phrase list is set. Send audio data stream.');
            readAndSendAudioFile(connection);
            break;
        case responseType.recognitionResult:
            if (messageJSON.status === 'recognized') {
                const current = Date.now();
                console.log(`${(current - pre) / 1000}:${messageJSON.value}`);
                pre = current;
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


const readAndSendAudioFile = (connection) => {
    fs.createReadStream(speechData.audioFile, { highWaterMark: 8192 })
        .on('data', (buf) => {
            //console.log(`Buffer length: ${buf.length}`);
            connection.send(buf, (error) => {
                if (error) {
                    console.error(error.message);
                }
            });
        }).on('end', () => {
            console.log('Audio data read finished');
            connection.send(JSON.stringify({
                command: commandType.endStream
            }));
        });
}

const main = async () => {
    const env = envConfigs.exeEnv;
    const { accessKey, secretKey } = env.authConfig;
    const tokenUrl = `${env.host.replace('ws', 'http')}${tokenPath}`;
    const token = await getJwtToken(tokenUrl, accessKey, secretKey);
    if (token) {
        const url = `${env.host}${apiPath}?token=Bearer ${token}`;
        console.log(url);
        const connection = new WebSocket(url);
        connection.on('open', () => {
            console.log('Connected to streaming STT API.');
            // Once connected, set the speech language.
            connection.send(JSON.stringify({
                command: commandType.setLanguage,
                value: speechData.language,
            }));
        });
        connection.on('message', (message) => {
            handleSessionMessage(connection, message);
        });
        connection.on('error', (code, reason) => {
            console.error(`Error: ${code}, ${reason}`);
        });
        connection.on('close', (code, reason) => {
            console.log(`Connection closed: ${code}, ${reason}`);
        });
    } else {
        console.error('JwtToken error');
    }

};

main();
