const superagent = require('superagent');
const fsp = require('fs').promises;
const fs = require('fs');
const path = require('path');
const unzipper = require('unzipper');
const config = require('./config');
const authUtils = require('./utils/auth-utils');

const translateItemIds = ['your translateItemId'];
const url = `/api/v1/downloads?ids=${JSON.stringify(translateItemIds)}`;

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
    }).end(async (req, resp) => {
      const zipFile = path.join(__dirname, 'sample-files', 'result.zip');
      await fsp.writeFile(zipFile, resp.body);
      fs.createReadStream(zipFile)
        .pipe(unzipper.Extract({ path: path.join(__dirname, 'sample-files') }));
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
