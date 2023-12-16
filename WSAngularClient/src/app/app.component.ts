import { Component } from '@angular/core';
import { WebSocketSubject, webSocket } from 'rxjs/webSocket';
import { Message } from './models/Message';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  Sender: string;
  Message: string = "";
  IsConected: boolean = false;
  Messages: Message[] = [];
  subject:WebSocketSubject<unknown>;

  OnConnect() {
    this.subject = webSocket('ws://tefash004-001-site1.gtempurl.com:80/ws?name=' + this.Sender);
    this.subject.subscribe({
      next: mgs => { this.Messages.push(mgs as Message); console.log(mgs); },
      error: error => { this.IsConected = false; console.log(error) },
      complete: () => { this.IsConected = false;console.log("Completed") }
    });
    this.IsConected = true;
  }
  OnDisconnect() {
    this.subject.unsubscribe();
    this.IsConected = false;
  }
  SendMessage() {
    let message = new Message();
    message.Sender = this.Sender;
    message.Text = this.Message;
    message.Date = new Date(Date.now());
    console.log(message);
    this.subject.next(message);
    this.Message = "";
  }
}
