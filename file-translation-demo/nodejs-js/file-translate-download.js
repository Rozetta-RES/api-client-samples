/**
* Demo program of download translated file.
* Command line: node ./file-translation-download.js translateItemId1,translateItemId2,translateItemId3....
* Required npm libs: superagent, crypto, adm-zip
*/
const superagent = require('superagent');
const crypto = require('crypto');
const fs = require('fs');
const AdmZip = require('adm-zip');

const { serverConfig } = require('./config');
const { authConfig } = require('./config');

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
* @param {string} translateItemIds Translate item ids to be downloaded, splitted by ','
*
* @returns {Promise<string>} Server response.
*
* @throws {Error} When unable to complete the request.
*/
const sendRequest = (serverConfig, authConfig, translateItemIds) => {
  const itemIds = translateItemIds.split(',')
  const url = `/api/v1/downloads?ids=${JSON.stringify(itemIds)}`;
  const nonce = new Date().getTime().toString();
  const signature = generateSignature(url, authConfig.secretKey, nonce);

  superagent.get(`${serverConfig.protocol}//${serverConfig.hostname}${url}`)
    .set({
      accessKey: authConfig.accessKey,
      signature,
      nonce,
    }).end(function (req, resp) {
      if (resp.status === 200) {
        fs.createWriteStream('./output.zip').write(resp.body, (error) => {
          if (error) {
            console.error(error);
          } else {
            const zip = new AdmZip('./output.zip');
            zip.extractAllTo('./', true);
          }
        });
      }
    });
};

const main = async () => {
  const translateItemIds = process.argv[2];
  if (!translateItemIds) {
    console.log("Error. please input translationItemId.");
    return;
  };
  try {
    await sendRequest(
      serverConfig,
      authConfig,
      translateItemIds
    );
  } catch (error) {
    console.error(error);
  }
};

main();
