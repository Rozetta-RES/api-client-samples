/**
 * Demo program for requesting a JWT from the Rozetta API.
 */

const fetch = require('node-fetch');

const URL = 'https://translate.classiii.io/api/v1/token';

const data = {
  accessKey: 'my-access-key',
  secretKey: 'my-secret-key',
  duration: 300
};

const getJWT = async () => {
  const response = await fetch(URL, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(data)
  });
  const responseJSON = await response.json();
  console.log(`JWT: ${responseJSON.data.encodedJWT}`);
};

getJWT();
