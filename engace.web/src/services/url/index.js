// export const DOMAIN_NAME = process.env.REACT_APP_API_URL || "http://localhost:3001/api";
export const DOMAIN_NAME = process.env.REACT_APP_API_URL || "https://japanace-api.onrender.com/api";
console.log(DOMAIN_NAME);

export const URL_GET_HEALCHECK = `${DOMAIN_NAME}/Healthcheck`;
export const URL_GET_JAPANESE_LEVEL = `${DOMAIN_NAME}/Quiz/GetJapaneseLevelss`;
export const URL_GET_DICTIONARY_SEARCH = `${DOMAIN_NAME}/Dictionary/Search`;
export const URL_GET_ESSAY_REVIEW = `${DOMAIN_NAME}/Review/Generate`;
export const URL_GET_CHAT_MESSAGE = `${DOMAIN_NAME}/Chatbot/GenerateAnswer`;
export const URL_GET_SUGGEST_TOPICS = `${DOMAIN_NAME}/Quiz/Suggest3Topics`;
export const URL_GET_QUIZ_TYPES = `${DOMAIN_NAME}/Quiz/GetQuizTypes`;
export const URL_GENERATE_QUIZ = `${DOMAIN_NAME}/Quiz/Generate`;

export const URL_GET_USER_INFO =
  "https://www.googleapis.com/oauth2/v3/userinfo";
