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
const secretKey = 'YOUR_SECRET_KEY';
const signature = generateSignature(path, secretKey, Math.floor(Date.now() / 1000));
console.log(signature);