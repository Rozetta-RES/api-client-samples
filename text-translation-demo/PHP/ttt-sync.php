<?php

/**
 * Synchronous text translation.
 */

const ACCESS_KEY = 'YOUR_ACCESS_KEY';
const SECRET_KEY = 'YOUR_SECRET_KEY';
const SIGNATURE_HMAC_ALGO = 'sha256';
const BASE_URL = 'https://translate.classiii.info';

/**
 * Generates authentication signature.
 */
function generate_signature(string $secret_key, string $nonce, string $path) {
  $context = hash_init(SIGNATURE_HMAC_ALGO, HASH_HMAC, $secret_key);
  hash_update($context, $nonce);
  hash_update($context, $path);
  return hash_final($context, FALSE);
}

/**
 * Sends a text translation request.
 */
function translate_text() {
  $path = '/api/v1/translate';
  $full_url = BASE_URL . $path;
  $nonce = time();
  $signature = generate_signature(SECRET_KEY, $nonce, $path);
  $headers = array(
    'Content-Type: application/json',
    'accessKey: ' . ACCESS_KEY,
    'nonce: ' . $nonce,
    'signature: ' . $signature,
  );
  $request_body = json_encode(
    array(
      'fieldId' => '1',
      'text' => array(
        '天気がいいから、散歩しましょう。',
        'このかばんはいくらですか？',
      ),
      'sourceLang' => 'ja',
      'targetLang' => 'en',
      'type' => 't4oo',
      'autoSplit' => TRUE,
      'removeFakeLineBreak' => FALSE,
    )
  );
  $options = array(
    'http' => array(
      'header'  => $headers,
      'method'  => 'POST',
      'content' => $request_body,
    ),
  );
  $context  = stream_context_create($options);
  $response = file_get_contents($full_url, false, $context);
  return $response;
}

function main() {
  $response = translate_text();
  $body_json = json_decode($response);
  foreach ($body_json->data->translationResult as $result) {
    echo 'Source text: ' . $result->sourceText . PHP_EOL;
    echo 'Translated text: ' . $result->translatedText . PHP_EOL;
  }
}

main();
