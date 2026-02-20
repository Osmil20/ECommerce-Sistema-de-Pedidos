import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <header class="header">
      <div class="brand">
        <span class="brand-icon">🛒</span>
        <span class="brand-name">ECommerce Admin</span>
      </div>
      <nav class="nav">
        <a routerLink="/pedidos" routerLinkActive="active">Pedidos</a>
        <a routerLink="/clientes" routerLinkActive="active">Clientes</a>
        <a routerLink="/produtos" routerLinkActive="active">Produtos</a>
      </nav>
    </header>

    <main class="main-content">
      <router-outlet />
    </main>
  `,
  styles: [`
    .header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0 32px;
      height: 60px;
      background: #1a1a2e;
      box-shadow: 0 2px 8px rgba(0,0,0,0.3);
      position: sticky;
      top: 0;
      z-index: 100;
    }
    .brand {
      display: flex;
      align-items: center;
      gap: 10px;
      color: #fff;
      font-size: 1.2rem;
      font-weight: 700;
    }
    .brand-icon { font-size: 1.5rem; }
    .nav {
      display: flex;
      gap: 8px;
    }
    .nav a {
      color: #aaa;
      text-decoration: none;
      padding: 8px 16px;
      border-radius: 6px;
      font-size: 0.9rem;
      font-weight: 500;
      transition: all 0.2s;
    }
    .nav a:hover, .nav a.active {
      color: #fff;
      background: rgba(255,255,255,0.1);
    }
    .nav a.active {
      color: #4fc3f7;
      background: rgba(79,195,247,0.15);
    }
    .main-content {
      max-width: 1200px;
      margin: 0 auto;
      padding: 32px 24px;
    }
  `]
})
export class AppComponent {}
