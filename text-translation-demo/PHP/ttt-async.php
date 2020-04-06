<?php

/**
 * Asynchronous text translation.
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
function request_text_translation() {
  $path = '/api/v1/translate/async';
  $full_url = BASE_URL . $path;
  $nonce = round(microtime(true) * 1000);
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

/**
 * Gets translation result.
 */
function get_translation_result(string $id) {
  $path = '/api/v1/translate/async/' . $id;
  $full_url = BASE_URL . $path;
  $nonce = round(microtime(true) * 1000);
  $signature = generate_signature(SECRET_KEY, $nonce, $path);
  $headers = array(
    'Content-Type: application/json',
    'accessKey: ' . ACCESS_KEY,
    'nonce: ' . $nonce,
    'signature: ' . $signature,
  );
  $options = array(
    'http' => array(
      'header'  => $headers,
      'method'  => 'GET',
    ),
  );
  $context  = stream_context_create($options);
  $response = file_get_contents($full_url, false, $context);
  return $response;
}

function main() {
  $response = request_text_translation();
  $translation_request_json = json_decode($response);
  $queue_id = $translation_request_json->data->queueId;
  echo 'Queue ID: ' . $queue_id . PHP_EOL;
  // Loop until the translation result is ready.
  while (TRUE) {
    $response = get_translation_result($queue_id);
    $translation_result_json = json_decode($response);
    $result_data = $translation_result_json->data;
    $hasMessage = property_exists($result_data, 'message');
    if ($hasMessage && ($result_data->message === 'In progress...')) {
      echo 'Translation in progress...' . PHP_EOL;
      sleep(1);
      continue;
    }
    foreach ($result_data->translationResult as $result) {
      echo 'Source text: ' . $result->sourceText . PHP_EOL;
      echo 'Translated text: ' . $result->translatedText . PHP_EOL;
    }
    break;
  }
}

main();
