/**
* Demo program of get user dictionary.
* Command line: node ./get-user-dict.js
* Required npm libs: superagent, crypto
*/
const superagent = require('superagent');
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
* @param {object} serverConfig Server configurations.
* @param {string} serverConfig.protocol Server protocol.
* @param {string} serverConfig.hostname Server hostname.
* @param {number} serverConfig.port Server listening port.
* @param {object} authConfig Authentication configurations.
* @param {string} authConfig.accessKey Access key.
* @param {string} authConfig.secretKey Secret key.
* @param {string} authConfig.nonce Nonce.
*
* @returns {Promise<string>} Server response.
*
* @throws {Error} When unable to complete the request.
*/
const sendRequest = (serverConfig, authConfig) => {
    const url = '/api/v1/dictionary';
    const signature = generateSignature(url, authConfig.secretKey, authConfig.nonce);

    const headers = {
        accessKey: authConfig.accessKey,
        signature,
        nonce: authConfig.nonce,
    }
    return superagent.get(`${serverConfig.protocol}//${serverConfig.hostname}${url}`)
        .set(headers)
        .then((res) => {
            return JSON.stringify(res.body);
        }).catch((err) => {
            return err.message;
        })
        
};

const main = async () => {
    try {
        const response = await sendRequest(serverConfig, authConfig);
        console.log('Server response:');
        console.log(response);
    } catch (error) {
        console.error(error);
    }
};

main();
