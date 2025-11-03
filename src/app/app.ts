import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from "./header/header";
import { Signup } from "./signup/signup";

@Component({
  selector: 'app-root',
  imports: [ Header, Signup],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('ProjetoLogin');
}
