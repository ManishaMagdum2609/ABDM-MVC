import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ABHA } from './Core/Features/Components/abha-Login/abhaLogin-component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css',
  
})
export class App {
  protected readonly title = signal('Frontend');
}
