export interface ApiError {
  detail: string;
  stackTrace: string;
  status: number;
  title: string;
  type: string;
  errors?: { [key: string]: string[] };
}
