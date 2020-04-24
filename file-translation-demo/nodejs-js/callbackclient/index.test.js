const index = require('./index');
const crypto = require('crypto');
const url = require('url');
const accessKey = "cad502e93e2e367aa3e3376aecf87a81f9d702213be85fe86ddf57f0c3f9b165";
const secretKey = "a82b5a3e48c177a02ff13b37c7e427e9f179a33f0a5e020715b455b6db012870fd58500ad877a00f239031159fff416e";
const apiPath= url.parse(index.API_URL).pathname
const CALLBACK_URL="http://223.132.67.129:3000/api/callback"
const generateSignature = (nonce, url, secretKey) => {
    const hmac = crypto.createHmac('sha256', secretKey);
    hmac.update(nonce);
    hmac.update(url);
    return hmac.digest('hex');
};
function isNullOrEmpty(value) {
    return value == null || value === "";
}
async function sleep(miliseconds) {
    await new Promise(r => setTimeout(r, miliseconds));
}
test('getCallbackUrl', async () => {
    await sleep(1);
    const nonce = Date. now().toString();
    const signature = generateSignature(nonce, apiPath, secretKey);
    const resp=await index.getCallback(accessKey, nonce,signature);
    expect(isNullOrEmpty(resp.data)).toBe(true);
});
test('putCallbackUrl', async () => {
    await sleep(1);
    const nonce = Date. now().toString();
    const signature = generateSignature(nonce, apiPath, secretKey);
    const bRet=await index.putCallback(accessKey, nonce,signature,CALLBACK_URL);
    expect(bRet).toBe(true);
    const resp=await index.getCallback(accessKey, nonce,signature);
    expect(resp.data).toBe(CALLBACK_URL);
});
test('deleteCallbackUrl', async () => {
    await sleep(1);
    const nonce = Date. now().toString();
    const signature = generateSignature(nonce, apiPath, secretKey);
    const bRet=await index.deleteCallback(accessKey, nonce,signature);
    expect(bRet).toBe(true);
    const resp=await index.getCallback(accessKey, nonce,signature);
    expect(isNullOrEmpty(resp.data)).toBe(true);
});
