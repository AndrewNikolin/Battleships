export class ChatMessage {
  author: string;
  message: string;

  constructor(values: Object = {}) {
    (<any>Object).assign(this, values);
  }

}
