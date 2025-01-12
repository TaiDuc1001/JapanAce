import * as UrlApi from "../url";
import axios from "axios";
import Cookies from "js-cookie";
import baseRequest from "./BaseRequest";

export const AppService = {
  healCheck: (token) => {
    return axios.get(UrlApi.URL_GET_HEALCHECK, {
      headers: {
        Authentication: token,
      },
    });
  },

  getUserInfo: () => {
    return axios.get(UrlApi.URL_GET_USER_INFO, {
      headers: {
        Authorization: `Bearer ${Cookies.get("token")}`,
      },
    });
  },

  getJapaneseLevels: () => {
    return baseRequest.get(UrlApi.URL_GET_JAPANESE_LEVEL);
  },

  getDictionarySearch: (keyword, context, useJapaneseToExplain) => {
    return baseRequest.get(
      `${UrlApi.URL_GET_DICTIONARY_SEARCH}?keyword=${encodeURIComponent(
        keyword
      )}&context=${encodeURIComponent(
        context ?? ""
      )}&useJapaneseToExplain=${encodeURIComponent(useJapaneseToExplain)}`
    );
  },

  getEssayReview: (content, level) => {
    return baseRequest.post(
      `${UrlApi.URL_GET_ESSAY_REVIEW}?JapaneseLevels=${level}`,
      content
    );
  },

  sendChatMessage: (chatHistory, question) => {
    const data = {
      ChatHistory: chatHistory,
      Question: question,
    };
    return baseRequest.post(UrlApi.URL_GET_CHAT_MESSAGE, data);
  },

  getSuggestTopics: (level) => {
    return baseRequest.get(
      `${UrlApi.URL_GET_SUGGEST_TOPICS}?JapaneseLevels=${level}`
    );
  },

  getQuizTypes: () => {
    return baseRequest.get(UrlApi.URL_GET_QUIZ_TYPES);
  },

  generateQuiz: (topic, qTypes, level, quantity) => {
    const data = {
      Topic: topic,
      QuizzTypes: qTypes,
    };
    return baseRequest.post(
      `${UrlApi.URL_GENERATE_QUIZ}?JapaneseLevels=${level}&totalQuestions=${quantity}`,
      data
    );
  },
};
