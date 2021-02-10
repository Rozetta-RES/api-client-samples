const recorder = require('node-record-lpcm16');
const WebSocket = require('ws');
const crypto = require('crypto');
const btoa = require('btoa');

const envConfigs = require('./account');
const apiPath = '/api/v1/translate/stt-streaming';
const speechData = {
  language: 'zh-CN',
  samplingRate: 16000,
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

const generateSignature = (path, secretKey, nonce) => {
  const hmac = crypto.createHmac(signatureHMACAlgo, secretKey);
  hmac.update(nonce);
  hmac.update(path);
  return hmac.digest(signatureHMACEncoding);
};

const getAuth = (authConfig, url) => {
  const nonce = Date.now().toString();
  return {
      accessKey: authConfig.accessKey,
      nonce: nonce,
      signature: generateSignature(url, authConfig.secretKey, nonce),
      remoteurl: url,
      contractId: authConfig.contractId
  }
}

//const file = fs.createWriteStream('temp.wav');
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
      recorder.record({
        sampleRate: 16000,
        threshold: 0.5,
        endOnSilence: true,
        silence: '15.0',
      }).stream()
        .on('data', (chunk) => {
          connection.send(chunk, (error) => {
            if (error) {
              console.error(error.message);
            }
          });
        })
        .on('end', () => {
          console.log('Recorder end');
        });
      break;
    case responseType.recognitionResult:
      if (messageJSON.status === 'recognized') {
        console.log(messageJSON.value);
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

const main = async () => {
  const env = envConfigs.stg2;
  const auth = getAuth(env.authConfig, apiPath);
  const auth64 = btoa(JSON.stringify(auth));
  const url = `${env.host}${apiPath}?auth=${auth64}`;
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
  connection.on('close', (code, message) => {
    console.log(`Connection closed.${code}, ${message}`);
  });
};

main();