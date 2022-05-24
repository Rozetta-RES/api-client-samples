const axios = require('axios');
const authUtils = require('./utils/auth-utils');

const path = '/api/v1/training/model/<mid>';

const authConfig = {
  accessKey: 'YOUR_ACCESS_KEY',
  secretKey: 'YOUR_SECRET_KEY',
  nonce: new Date().getTime().toString()
};

const signature = authUtils.generateSignature(
  authConfig.nonce,
  path,
  authConfig.secretKey,
);

const config = {
  method: 'get',
  url: 'https://translate.rozetta-api.io/api/v1/training/model/<mid>',
  headers: {
    accessKey: authConfig.accessKey,
    signature,
    nonce: authConfig.nonce,
    'Content-Type': 'application/json',
  },
};

const main = async () => {
  try {
    const response = await axios(config)
    console.log(JSON.stringify(response.data));
  } catch (error) {
    console.log(error);
  }
}

main();
