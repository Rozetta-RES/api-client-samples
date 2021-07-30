
/**
* Command type sent from the client.
*/
const commandType = {
    setLanguage: 'SET_LANGUAGE',
    setSamplingRate: 'SET_SAMPLING_RATE',
    endStream: 'END_STREAM',
    endSession: 'END_SESSION',
    setPhraseList: 'SET_PHRASE_LIST',
};

/**
* Response types received from API endpoint.
*/
const responseType = {
    languageReady: 'LANGUAGE_READY',
    languageError: 'SET_LANGUAGE_ERROR',
    samplingRateReady: 'SAMPLING_RATE_READY',
    samplingRateError: 'SET_SAMPLING_RATE_ERROR',
    recognitionResult: 'RECOGNITION_RESULT',
    recognitionError: 'RECOGNITION_ERROR',
    phraseListReady: 'PHRASE_LIST_READY',
    phraseListError: 'PHRASE_LIST_ERROR',
};

module.exports = {
    commandType,
    responseType
};