const superagent = require('superagent');
const fs = require('fs');
const path = require('path');
const config = require('./config');

const authUtils = require('./utils/auth-utils');

const url = '/api/v1/file-translate';

const sendRequest = async (serverConfig, accessKey, secretKey) => {
  const nonce = new Date().getTime().toString();
  const signature = authUtils.generateSignature(
    url,
    secretKey,
    nonce,
  );
  const langs = ['en'];
  superagent.post(`${serverConfig.protocol}//${serverConfig.hostname}:${serverConfig.port}${url}`)
    .set({
      accessKey,
      signature,
      nonce,
    })
    .attach('files', fs.createReadStream(path.join(
      __dirname,
      'sample-files',
      'testfile.docx',
    )), 'testfile.docx')
    .field('targetLangs', JSON.stringify(langs))
    .field('fieldId', '1')
    .end((_, resp) => {
      console.log(resp.text);
    });
};

const main = async () => {
  try {
    await sendRequest(
      config.serverConfig,
      config.authConfig.accessKey,
      config.authConfig.secretKey,
    );
  } catch (error) {
    console.error(error);
  }
};

main();
