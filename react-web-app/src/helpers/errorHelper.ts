import { ApiError } from 'models';

const isApiError = (err: unknown) => {
  const apiError = err as ApiError;
  return (
    apiError.title !== undefined &&
    apiError.detail !== undefined &&
    apiError.status !== undefined &&
    apiError.type !== undefined
  );
};

export const ErrorHelper = {
  formatErrorMessage(error: unknown) {
    let errorMessage = '';
    if (isApiError(error)) {
      const apiError = error as ApiError;
      if (apiError.status === 422) {
        errorMessage = apiError.title;
        if (apiError.errors) {
          errorMessage = errorMessage.concat(': ');
          Object.entries(apiError.errors).forEach(([key, value]) => {
            errorMessage = errorMessage.concat(` ${value.join('; ')}`);
          });
        }
      } else {
        errorMessage = apiError.detail;
      }
    } else {
      const err = error as Error;
      if (err?.message != null) {
        errorMessage = err.message;
      } else {
        errorMessage = 'Unknown error';
      }
    }

    return errorMessage;
  },
};
