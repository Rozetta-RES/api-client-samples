
const superagent = require('superagent');

const authUtils = require('./utils/auth-utils');

const serverConfig = {
  protocol: 'https:',
  hostname: 'translate.classiii.io',
  port: 443,
};

const authConfig = {
  accessKey: 'YOUR_ACCESSKEY',
  secretKey: 'YOUR_SECRETKEY',
  transId: 'transID',
};

const url = `/api/v1/translate-result/${authConfig.transId}`;

const sendRequest = (accessKey, secretKey) => {
  const nonce = new Date().getTime().toString();
  const signature = authUtils.generateSignature(
    url,
    secretKey,
    nonce,
  );

  superagent.get(`${serverConfig.protocol}//${serverConfig.hostname}${url}`)
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
      serverConfig,
      authConfig.accessKey,
      authConfig.secretKey,
    );
  } catch (error) {
    console.error(error);
  }
};

main();
