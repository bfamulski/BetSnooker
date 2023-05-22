import { httpClient } from './httpClient';
import { Credentials, User } from 'models';

const BaseUrl = process.env.REACT_APP_BASE_API_URL;

export const authenticationService = {
  async login(credentials: Credentials) {
    // TODO: credentials are seen on browser, maybe encode it?
    return await httpClient.post<User, Credentials>(`${BaseUrl}/authentication/login`, credentials);
  },
};
