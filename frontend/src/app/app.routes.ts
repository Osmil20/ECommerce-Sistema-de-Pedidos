import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: '/pedidos', pathMatch: 'full' },
  {
    path: 'clientes',
    loadComponent: () =>
      import('./components/clientes/clientes.component').then(m => m.ClientesComponent)
  },
  {
    path: 'produtos',
    loadComponent: () =>
      import('./components/produtos/produtos.component').then(m => m.ProdutosComponent)
  },
  {
    path: 'pedidos',
    loadComponent: () =>
      import('./components/pedidos/pedidos.component').then(m => m.PedidosComponent)
  },
  {
    path: 'pedidos/novo',
    loadComponent: () =>
      import('./components/pedidos/novo-pedido.component').then(m => m.NovoPedidoComponent)
  }
];
