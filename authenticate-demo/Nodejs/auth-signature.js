/**
 * Demo program for generating authentication signature.
 */
const authUtils = require('./utils/auth-utils');

const authConfig = {
  accessKey: 'YOUR_ACCESSKEY',
  secretKey: 'YOUR_SECRETKEY',
};

const path = '/api/v1/translate';

const main = () => {
  const nonce = new Date().getTime().toString();
  const signature = authUtils.generateSignature(path, authConfig.secretKey, nonce);
  console.log(`Access key: ${authConfig.accessKey}`);
  console.log(`Signature: ${signature}`);
  console.log(`Nonce: ${nonce}`);
  console.log(`Path: ${path}`);
};

main();
