/**
 * Speech-to-text demo program.
 */

const FormData = require('form-data');

const fs = require('fs');
const crypto = require('crypto');

const serverConfig = {
  protocol: 'https:',
  hostname: 'staging1.classiii.info',
  port: 443
};
const authConfig = {
  accessKey: 'ACCESS_KEY',
  secretKey: 'SECRET_KEY',
  nonce: Date.now().toString()
};
const speechConfig = {
  lang: 'ja',
  audioFile: 'speech.wav'
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

/**
 * Sends request for speech-to-speech translation.
 *
 * @param {object} serverConfig Server configurations.
 * @param {string} serverConfig.protocol Server protocol.
 * @param {string} serverConfig.hostname Server hostname.
 * @param {number} serverConfig.port Server listening port.
 * @param {object} authConfig Authentication configurations.
 * @param {string} authConfig.accessKey Access key.
 * @param {string} authConfig.secretKey Secret key.
 * @param {string} authConfig.nonce Nonce.
 * @param {object} speechConfig Speech configurations.
 * @param {string} speechConfig.lang Speech language.
 * @param {string} speechConfig.audioFile Speech file path.
 *
 * @returns {Promise<string>} Server response.
 *
 * @throws {Error} When unable to complete the request.
 */
const sendRequest = (serverConfig, authConfig, speechConfig) => {
  const form = new FormData();
  form.append('sourceLang', speechConfig.lang);
  form.append('audioFile', fs.createReadStream(speechConfig.audioFile));
  const path = '/api/v1/translate/stt';
  const signature = generateSignature(
    path,
    authConfig.secretKey,
    authConfig.nonce
  );
  return new Promise((resolve, reject) => {
    form.submit(
      {
        protocol: serverConfig.protocol,
        host: serverConfig.hostname,
        port: serverConfig.port,
        path,
        headers: {
          accessKey: authConfig.accessKey,
          signature,
          nonce: authConfig.nonce
        }
      },
      (error, response) => {
        if (error !== null) {
          reject(error);
          return;
        }
        response.setEncoding('utf8');
        let data = '';
        response.on('data', (chunk) => {
          data += chunk;
        });
        response.on('end', () => {
          resolve(data);
        });
      }
    );
  });
};

const main = async () => {
  try {
    const response = await sendRequest(
      serverConfig,
      authConfig,
      speechConfig
    );
    console.log('Server response:');
    console.log(response);
  } catch (error) {
    console.error(error);
  }
};

main();