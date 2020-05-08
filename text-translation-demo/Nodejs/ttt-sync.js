'use strict';

const https = require('https');

const authUtils = require('./utils/auth-utils');

const serverConfig = {
  protocol: 'https:',
  hostname: 'translate.classiii.io',
  port: 443
};
const authConfig = {
  accessKey: 'ACCESS_KEY',
  secretKey: 'SECRET_KEY'
};
const translationData = {
  fieldId: '1',
  text: [
    'Hello'
  ],
  sourceLang: 'en',
  targetLang: 'ja',
  type: 't4oo',
};

/**
 * Gets translation result as text.
 *
 * @param {object} serverConfig Server configurations.
 * @param {string} serverConfig.protocol Server protocol.
 * @param {string} serverConfig.hostname Server hostname.
 * @param {number} serverConfig.port Server listening port.
 * @param {object} authConfig Authentication configurations.
 * @param {string} authConfig.accessKey Access key.
 * @param {string} authConfig.secretKey Secret key.
 * @param {object} translationData Translation data.
 * @param {string} translationData.field Professional field ID.
 * @param {string[]} translationData.text Texts to be translated.
 * @param {string} translationData.sourceLang Language code of language of
 *                                            original text.
 * @param {string} translationData.targetLang Language code of language of
 *                                            translated text.
 *
 * @returns {Promise<string>} A Promise resolved with the server response.
 */
const getTextResult = (serverConfig, authConfig, translationData) => {
  const path = '/api/v1/translate';
  // We use UNIX time (in milliseconds) as nonce here.
  const nonce = `${Math.floor(Date.now())}`;
  const signature = authUtils.generateSignature(
    path,
    authConfig.secretKey,
    nonce
  );
  const requestOptions = {
    protocol: serverConfig.protocol,
    host: serverConfig.hostname,
    port: serverConfig.port,
    method: 'POST',
    path,
    headers: {
      accessKey: authConfig.accessKey,
      signature,
      nonce,
      'Content-Type': 'application/json'
    }
  };
  return new Promise((resolve, reject) => {
    // You can also use 3rd party modules, such as superagent, to send the
    // request. Here we use the Node.js built-in https module.
    const request = https.request(requestOptions, (response) => {
      response.setEncoding('utf8');
      console.log(`Status code: ${response.statusCode}`);
      let data = '';
      response.on('data', (chunk) => {
        data += chunk;
      });
      response.on('end', () => {
        resolve(data);
      });
    });
    request.on('error', (error) => {
      reject(error);
    });
    request.write(JSON.stringify(translationData));
    request.end();
  });
};

const main = async () => {
  try {
    const response = await getTextResult(
      serverConfig,
      authConfig,
      translationData
    );
    console.log('Server response:');
    console.log(response);
  } catch (error) {
    console.error(error);
  }
};

main();
