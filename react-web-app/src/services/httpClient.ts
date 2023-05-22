import { ApiError } from 'models';

const handleResponseError = async (response: Response): Promise<ApiError | Error> => {
  try {
    const responseJson = await response.json();
    const error = responseJson as ApiError;
    return error;
  } catch (error) {
    // fallback
    return new Error('HTTP request error - status: ' + response.status);
  }
};

const fetchWithResponse = async <TResponse>(
  url: string,
  config: globalThis.RequestInit = {}
): Promise<TResponse | null> => {
  const response = await fetch(url, config);
  if (response.ok) {
    const responseJson = await response.json();
    return responseJson as TResponse;
  } else {
    throw await handleResponseError(response);
  }
};

export const httpClient = {
  async get<TResponse>(url: string) {
    return await fetchWithResponse<TResponse>(url);
  },
  async post<TResponse, TBody>(url: string, body: TBody) {
    return await fetchWithResponse<TResponse>(url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json;charset=UTF-8' },
      body: JSON.stringify(body),
    });
  },
};

export const authHttpClient = {
  async get<TResponse>(url: string, accessToken: string) {
    return await fetchWithResponse<TResponse>(url, { headers: { Authorization: `Bearer ${accessToken}` } });
  },
  async post<TResponse, TBody>(url: string, accessToken: string, body: TBody) {
    return await fetchWithResponse<TResponse>(url, {
      method: 'POST',
      headers: { Authorization: `Bearer ${accessToken}`, 'Content-Type': 'application/json;charset=UTF-8' },
      body: JSON.stringify(body),
    });
  },
};
