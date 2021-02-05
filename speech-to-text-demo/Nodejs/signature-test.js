const crypto = require('crypto');

const signatureHMACAlgo = 'sha256';
const signatureHMACEncoding = 'hex';
const generateSignature = (path, secretKey, nonce) => {
    const hmac = crypto.createHmac(signatureHMACAlgo, secretKey);
    hmac.update(nonce);
    hmac.update(path);
    return hmac.digest(signatureHMACEncoding);
};

const path = '/api/vi/translate';
const secretKey = '89919ee94177e36ea730205d36b407ce7f5cf582dfd6a793818901a538a6ee2eb37d5d05e6a31572468c89d138d92476';
const signature = generateSignature(path, secretKey, '1611844759002367');
console.log(signature);