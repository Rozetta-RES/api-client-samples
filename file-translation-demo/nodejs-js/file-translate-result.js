const superagent = require('superagent');
const config = require('./config');

const authUtils = require('./utils/auth-utils');

const translateId = 'your translateId';
const url = `/api/v1/translate-result/${translateId}`;

const sendRequest = (serverConfig, accessKey, secretKey) => {
  const nonce = new Date().getTime().toString();
  const signature = authUtils.generateSignature(
    url,
    secretKey,
    nonce,
  );

  superagent.get(`${serverConfig.protocol}//${serverConfig.hostname}:${serverConfig.port}${url}`)
    .set({
      accessKey,
      signature,
      nonce,
    }).end((req, resp) => {
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
