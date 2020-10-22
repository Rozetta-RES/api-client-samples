'use strict';

/**
 * Streaming speech-to-text demo program.
 */

const fs = require('fs');
const WebSocket = require('ws');
const crypto = require('crypto');

const fsPromise = fs.promises;

const apiPath = '/api/v1/translate/stt-streaming';
const apiEndpoint = `ws://localhost:3000${apiPath}`;
const authConfig = {
  accessKey: 'ACCESS_KEY',
  secretKey: 'SECRET_KEY',
  nonce: Date.now().toString(),
  contractId: 'CONTRACT_ID',
};
const speechData = {
  language: 'zh-CN',
  samplingRate: 16000,
  audioFile: 'zh_cn.wav',
  audioBuffer: null,
};

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
      connection.send(speechData.audioBuffer, (error) => {
        if (error) {
          console.error(error.message);
        }
        connection.send(JSON.stringify({
          command: commandType.endStream,
        }));
      });
      break;
    case responseType.recognitionResult:
      console.log('Recognized transcript:');
      console.log(messageJSON.value);
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
  speechData.audioBuffer = await fsPromise.readFile(speechData.audioFile);
  const auth = AuthUtil.getAuth(this._apiPath);
  const auth64 = btoa(JSON.stringify(auth));
  const url = `${apiEndpoint}?auth=${auth64}`
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
  connection.on('error', (error) => {
    console.error(error.message);
    connection.close();
  });
  connection.on('close', () => {
    console.log('Connection closed.');
  });
};

main();
