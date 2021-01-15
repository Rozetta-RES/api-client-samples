'use strict';

/**
 * Streaming speech-to-text demo program.
 */

const fs = require('fs');
const WebSocket = require('ws');
const crypto = require('crypto');
const btoa = require('btoa');

const fsPromise = fs.promises;

const apiPath = '/api/v1/translate/stt-streaming';
const apiEndpoint = `wss://translate.classiii.io${apiPath}`;
const authConfig = {
  accessKey: 'dc9cd4ac9a11639ba404273d7a702ffc0694b24ab42108c1cd4be7f4676535d8',
  secretKey: 'cae7b345386cf95863de78b9ce9552bc5bbf14aecac5412d041aaf8bbb6bf4b81223809dc64093093a3e3102cd97f833',
  nonce: Date.now().toString(),
  contractId: '424367c0-ec1a-11ea-b2a3-83b1a2aaa1f2',
};
const speechData = {
  language: 'ja',
  samplingRate: 16000,
  audioFile: 'ja_jp.wav'
};

const start = Date.now();
/**
* Command type sent from the client.
*/
const commandType = {
  setLanguage: 'SET_LANGUAGE',
  setSamplingRate: 'SET_SAMPLING_RATE',
  endStream: 'END_STREAM',
  endSession: 'END_SESSION',
};

/**
* Response types received from API endpoint.
*/
const responseType = {
  languageReady: 'LANGUAGE_READY',
  samplingRateReady: 'SAMPLING_RATE_READY',
  recognitionResult: 'RECOGNITION_RESULT',
  recognitionError: 'RECOGNITION_ERROR',
};

const signatureHMACAlgo = 'sha256';
const signatureHMACEncoding = 'hex';

/**
 * Generates a request signature.
 *
 * @param {string} path Path.
 * @param {string} secretKey Secret key.
 * @param {string} nonce Nonce.
 *
 * @returns {string} The request signature.
 */
const generateSignature = (path, secretKey, nonce) => {
  const hmac = crypto.createHmac(signatureHMACAlgo, secretKey);
  hmac.update(nonce);
  hmac.update(path);
  return hmac.digest(signatureHMACEncoding);
};

const getAuth = (url) => {
  const nonce = Date.now().toString();
  return {
      accessKey: authConfig.accessKey,
      nonce: nonce,
      signature: generateSignature(url, authConfig.secretKey, nonce),
      remoteurl: url,
      contractId: authConfig.contractId
  }
}

const handleSessionMessage = (connection, message) => {
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
      console.log(`Recognized transcript:${Date.now() - start} milliseconds`);
      console.log(messageJSON);
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

const main = async () => {
  const auth = getAuth(apiPath);
  const auth64 = btoa(JSON.stringify(auth));
  const url = `${apiEndpoint}?auth=${auth64}`
  console.log(url);
  const connection = new WebSocket(url);
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
    handleSessionMessage(connection, message);
  });
  connection.on('error', (error) => {
    console.error(error.message);
    connection.close();
  });
  connection.on('close', () => {
    console.log('Connection closed.');
  });
};

main();
