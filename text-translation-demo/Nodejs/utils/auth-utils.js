/**
 * Authentication utilities module.
 */
const crypto = require('crypto');

const signatureHMACAlgo = 'sha256';
const signatureHMACEncoding = 'hex';

const serverConfig = {
  protocol: 'https:',
  hostname: 'translate.classiii.io',
  port: 443
};
const authConfig = {
  accessKey: 'ACCESS_KEY',
  secretKey: 'SECRET_KEY'
};

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

module.exports = {
  serverConfig,
  authConfig,
  generateSignature,
};
