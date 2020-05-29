const axios = require("axios");
const API_URL="https://translate.classiii.io/api/v1/callback";
async function putCallback(accessKey, nonce, signature, callbackUrl) {
    const resp = await axios({
        method: 'put',
        url: API_URL,
        timeout: 10000,
        headers: {accessKey,nonce, signature},
        data: {callbackUrl: callbackUrl}
    });
    if (resp.status >= 200 && resp.status < 300)
        return true;
    return false;
}
async function getCallback(accessKey, nonce, signature) {
    const resp = await axios({
        method: 'get',
        url: API_URL,
        timeout: 10000,
        headers: {accessKey,nonce, signature},
    });
    if (resp.status >= 200 && resp.status < 300)
        return resp.data;
    return null;
}
async function deleteCallback(accessKey, nonce, signature) {
    const resp = await axios({
        method: 'delete',
        url: API_URL,
        timeout: 10000,
        headers: {accessKey,nonce, signature},
    });
    if (resp.status >= 200 && resp.status < 300)
        return true;
    return false;
}
module.exports = {
    putCallback,
    getCallback,
    deleteCallback,
    API_URL
}
