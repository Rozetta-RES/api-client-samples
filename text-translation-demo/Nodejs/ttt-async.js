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
    'Good morning.'
  ],
  sourceLang: 'en',
  targetLang: 'ja',
  type: 't4oo'
};

/**
 * Waits for a specific period of time.
 *
 * @param {number} duration Duration in milliseconds.
 */
const sleep = (duration) => new Promise((resolve) => {
  setTimeout(() => {
    resolve();
  }, duration);
});

/**
 * Submits translation task.
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
 * @returns {Promise<string>} A Promise resolved with the translation ID.
 *
 * @throws {Error} When unable to complete the request.
 */
const submitTranslation = (serverConfig, authConfig, translationData) => {
  const path = '/api/v1/translate/async';
  // We use UNIX time (in milliseconds) as nonce here.
  const nonce = `${Date.now()}`;
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
      let data = '';
      response.on('data', (chunk) => {
        data += chunk;
      });
      response.on('end', () => {
        const dataJSON = JSON.parse(data);
        resolve(dataJSON.data.queueId);
      });
    });
    request.on('error', (error) => {
      reject(error);
    });
    request.write(JSON.stringify(translationData));
    request.end();
  });
};

/**
 * Gets translation result.
 *
 * @param {object} serverConfig Server configurations.
 * @param {string} serverConfig.protocol Server protocol.
 * @param {string} serverConfig.hostname Server hostname.
 * @param {number} serverConfig.port Server listening port.
 * @param {object} authConfig Authentication configurations.
 * @param {string} authConfig.accessKey Access key.
 * @param {string} authConfig.secretKey Secret key.
 * @param {string} translationId Translation ID.
 *
 * @returns {Promise<string>} A Promise resolved with the server response.
 *
 * @throws {Error} When unable to complete the request.
 */
const getTranslationResult = (serverConfig, authConfig, translationId) => {
  const path = `/api/v1/translate/async/${translationId}`;
  // We use UNIX time (in milliseconds) as nonce here.
  const nonce = `${Date.now()}`;
  const signature = authUtils.generateSignature(
    path,
    authConfig.secretKey,
    nonce
  );
  const requestOptions = {
    protocol: serverConfig.protocol,
    host: serverConfig.hostname,
    port: serverConfig.port,
    method: 'GET',
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
      console.log(`Status code: ${response.statusCode}`);
      response.setEncoding('utf8');
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
    request.end();
  });
};

const main = async () => {
  try {
    const translationId = await submitTranslation(
      serverConfig,
      authConfig,
      translationData
    );
    console.log(`Translation ID: ${translationId}`);
    while (true) {
      const response = await getTranslationResult(
        serverConfig,
        authConfig,
        translationId
      );
      console.log('Server response:');
      console.log(response);
      const responseJSON = JSON.parse(response);
      if (responseJSON.status === 'failure') {
        break;
      }
      if (Array.isArray(responseJSON.data.translationResult)) {
        break;
      }
      await sleep(1000);
    }
  } catch (error) {
    console.error(error);
  }
};

main();
