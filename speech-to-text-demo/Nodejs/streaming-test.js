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
const apiEndpoint = `wss://staging1.classiii.info${apiPath}`;
const authConfig = {
  accessKey: 'a91ca9a65a076af060caf1a5dd2efb57b300ae4c68454a09f9fc72222f95d71e',
  secretKey: '8e211d4d64920c8169c6f413f73baa706b1625b668d31d5f94a81c0ddeea18b391432753f7a04dc82b5d05d753998608',
  nonce: Date.now().toString(),
  contractId: '12b7fa50-d0b0-11ea-ad62-b568e8b6e6f6',
};
const speechData = {
  language: 'ja',
  samplingRate: 16000,
  audioFile: 'user01-02.wav'
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
