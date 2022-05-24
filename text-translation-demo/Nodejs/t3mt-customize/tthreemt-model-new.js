const axios = require('axios');
const FormData = require('form-data');
const fs = require('fs');

const data = new FormData();

const authUtils = require('./../utils/auth-utils');

data.append('file', fs.createReadStream('YOUR_FILE_PATH'));
data.append('name', 'Sample');
data.append('firstLang', 'en');
data.append('secondLang', 'ja');
data.append('field', 'L120001');
data.append('contractId', 'YOUR_CONTRACT_ID');

const authConfig = {
  accessKey: 'YOUR_ACCESS_KEY',
  secretKey: 'YOUR_SECRET_KEY',
  nonce: new Date().getTime().toString()
};

const path = '/api/v1/training/train';

const signature = authUtils.generateSignature(
  authConfig.nonce,
  path,
  authConfig.secretKey,
);

const config = {
  method: 'post',
  url: 'https://translate.rozetta-api.io/api/v1/training/train',
  headers: {
    accessKey: authConfig.accessKey,
    signature,
    nonce: authConfig.nonce,
    'Content-Type': 'multipart/form-data',
    ...data.getHeaders()
  },
  data : data
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
