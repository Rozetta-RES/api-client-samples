/**
 * Demo prorgram of authentication by JWT.
 */

const fetch = require('node-fetch');

const token = 'my-jwt';

const sampleRequest = async () => {
  const url = 'https://translate.classiii.io/api/v1/hello';
  const response = await fetch(url, {
    method: 'GET',
    headers: {
      Authorization: `Bearer ${token}`
    }
  });
  const responseJSON = await response.json();
  console.log(responseJSON);
};

sampleRequest();
