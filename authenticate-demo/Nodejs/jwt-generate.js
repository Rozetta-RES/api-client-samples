/**
 * Demo program for generating JWT (JSON Web Token).
 */

const jwt = require('jsonwebtoken');

const TOKEN_SIGNING_OPTIONS = {
  algorithm: 'HS256',
};

const userId = 'MyUserID';
const accessKey = 'my-access-key';
const secretKey = 'my-secret-key';
const validDuration = 60 * 30;

const payload = {
  exp: Math.floor(Date.now() / 1000) + validDuration,
  iss: userId,
  accessKey,
};

const encodedJWT = jwt.sign(payload, secretKey, TOKEN_SIGNING_OPTIONS);

console.log(`JWT: ${encodedJWT}`);
