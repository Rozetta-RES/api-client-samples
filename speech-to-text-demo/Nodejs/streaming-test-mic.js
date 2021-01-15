const recorder = require('node-record-lpcm16');
const WebSocket = require('ws');
const crypto = require('crypto');
const btoa = require('btoa');

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
        recorder.record({
          endOnSilence: true,
          silence: '10.0',
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
    const auth = getAuth(apiPath);
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
      recorder.stop();
    });
};

/*
const main = async () => {
  const WebSocketClient = require('websocket').client
  let client = new WebSocketClient()
  client.on('connect', function (connection) {
    console.log('Connected')
    connection.on('error', function (error) {
      console.log('Connection Error: ' + error.toString())
    })
    connection.on('close', function () {
      console.log('Connection Closed')
    })
    connection.on('message', function (message) {
      if (message.type === 'utf8') {
        console.log('Received:', message.utf8Data)
      }
    })
    recorder
        .record({
          sampleRate: 16000,
          threshold: 0.5,
          endOnSilence: true,
          silence: '15.0',
        })
        .stream()
        .on('data', (chunk) => {
          connection.sendUTF(JSON.stringify(chunk))
        })
        .on('end', () => {
          console.log('Recorder end');
          recognizer.end();
        });
  })
  
  client.connect("wss://api.stt.dandi-poc-1.poc-dev.com" + '?uid=alt-aigijiroku', 'echo-protocol')
}
*/
main();