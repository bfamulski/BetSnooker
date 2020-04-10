export class User {
  id: number;
  username: string;
  password: string;
  firstName: string;
  lastName: string;
  authdata?: string;

  public constructor(fields?: Partial<User>) {
    Object.assign(this, fields);
  }
}
