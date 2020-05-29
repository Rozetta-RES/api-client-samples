/**
 * Demo program for generating authentication signature.
 */

const crypto = require('crypto');

const signatureHMACAlgo = 'sha256';
const signatureHMACEncoding = 'hex';


const authConfig = {
  accessKey: 'YOUR_ACCESSKEY',
  secretKey: 'YOUR_SECRETKEY',
};

const path = '/api/v1/translate';

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

const main = () => {
  const nonce = new Date().getTime().toString();
  const signature = generateSignature(path, authConfig.secretKey, nonce);
  console.log(`Access key: ${authConfig.accessKey}`);
  console.log(`Signature: ${signature}`);
  console.log(`Nonce: ${nonce}`);
  console.log(`Path: ${path}`);
};

main();
