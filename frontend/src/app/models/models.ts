export interface Cliente {
  id: number;
  nome: string;
  cpf: string;
  email: string;
  criadoEm: string;
}

export interface ClienteCreate {
  nome: string;
  cpf: string;
  email: string;
}

export interface Produto {
  id: number;
  nome: string;
  preco: number;
  descricao: string;
  estoque: number;
}

export interface ProdutoCreate {
  nome: string;
  preco: number;
  descricao: string;
  estoque: number;
}

export interface ItemPedido {
  produtoId: number;
  quantidade: number;
}

export interface PedidoCreate {
  clienteId: number;
  itens: ItemPedido[];
}

export interface PedidoItem {
  produtoId: number;
  nomeProduto: string;
  quantidade: number;
  precoUnitario: number;
  subtotal: number;
}

export interface Pedido {
  id: number;
  clienteId: number;
  nomeCliente: string;
  status: 'Criado' | 'Pago' | 'Cancelado';
  dataPedido: string;
  dataAtualizacao: string | null;
  valorTotal: number;
  itens: PedidoItem[];
}

export interface Historico {
  id: number;
  entidade: string;
  entidadeId: number;
  operacao: string;
  dadosAnteriores: string;
  dadosNovos: string;
  realizadoEm: string;
  realizadoPor: string;
}
