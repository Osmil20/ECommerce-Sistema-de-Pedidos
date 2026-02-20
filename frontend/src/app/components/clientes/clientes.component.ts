import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Cliente } from '../../models/models';
import { ClienteService } from '../../services/cliente.service';

@Component({
  selector: 'app-clientes',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="page">
      <div class="page-header">
        <h1>Clientes</h1>
        <button class="btn btn-primary" (click)="abrirFormulario()">+ Novo Cliente</button>
      </div>

      @if (exibirForm) {
        <div class="card form-card">
          <h2>{{ editandoId ? 'Editar Cliente' : 'Novo Cliente' }}</h2>
          <form [formGroup]="form" (ngSubmit)="salvar()">
            <div class="form-row">
              <div class="form-group">
                <label>Nome *</label>
                <input formControlName="nome" placeholder="Nome completo" />
                @if (form.get('nome')?.invalid && form.get('nome')?.touched) {
                  <span class="error">Nome é obrigatório</span>
                }
              </div>
              <div class="form-group">
                <label>CPF *</label>
                <input formControlName="cpf" placeholder="000.000.000-00" [readonly]="!!editandoId" />
              </div>
              <div class="form-group">
                <label>E-mail *</label>
                <input formControlName="email" placeholder="email@exemplo.com" type="email" />
              </div>
            </div>
            <div class="form-actions">
              <button type="button" class="btn btn-secondary" (click)="cancelar()">Cancelar</button>
              <button type="submit" class="btn btn-primary" [disabled]="form.invalid">
                {{ editandoId ? 'Salvar' : 'Cadastrar' }}
              </button>
            </div>
          </form>
        </div>
      }

      @if (erro) {
        <div class="alert alert-error">{{ erro }}</div>
      }
      @if (sucesso) {
        <div class="alert alert-success">{{ sucesso }}</div>
      }

      <div class="card">
        <table class="table">
          <thead>
            <tr>
              <th>#</th>
              <th>Nome</th>
              <th>CPF</th>
              <th>E-mail</th>
              <th>Cadastrado em</th>
              <th>Ações</th>
            </tr>
          </thead>
          <tbody>
            @for (c of clientes; track c.id) {
              <tr>
                <td>{{ c.id }}</td>
                <td><strong>{{ c.nome }}</strong></td>
                <td>{{ c.cpf }}</td>
                <td>{{ c.email }}</td>
                <td>{{ c.criadoEm | date:'dd/MM/yyyy' }}</td>
                <td>
                  <button class="btn-icon" (click)="editar(c)" title="Editar">✏️</button>
                  <button class="btn-icon btn-danger" (click)="remover(c.id)" title="Remover">🗑️</button>
                </td>
              </tr>
            }
            @if (clientes.length === 0) {
              <tr><td colspan="6" class="empty">Nenhum cliente cadastrado.</td></tr>
            }
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: [`
    
  `]
})
export class ClientesComponent implements OnInit {
  clientes: Cliente[] = [];
  form!: FormGroup;
  exibirForm = false;
  editandoId: number | null = null;
  erro = '';
  sucesso = '';

  constructor(private service: ClienteService, private fb: FormBuilder) {}

  ngOnInit(): void {
    this.initForm();
    this.carregar();
  }

  initForm(): void {
    this.form = this.fb.group({
      nome: ['', Validators.required],
      cpf: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]]
    });
  }

  carregar(): void {
    this.service.listar().subscribe({ next: c => this.clientes = c });
  }

  abrirFormulario(): void {
    this.editandoId = null;
    this.form.reset();
    this.form.get('cpf')?.enable();
    this.exibirForm = true;
  }

  editar(c: Cliente): void {
    this.editandoId = c.id;
    this.form.patchValue({ nome: c.nome, cpf: c.cpf, email: c.email });
    this.form.get('cpf')?.disable();
    this.exibirForm = true;
    this.erro = '';
    this.sucesso = '';
  }

  salvar(): void {
    if (this.form.invalid) return;
    const { nome, cpf, email } = this.form.getRawValue();

    if (this.editandoId) {
      this.service.atualizar(this.editandoId, { nome, email }).subscribe({
        next: () => { this.sucesso = 'Cliente atualizado!'; this.cancelar(); this.carregar(); },
        error: () => { this.erro = 'Erro ao atualizar cliente.'; }
      });
    } else {
      this.service.criar({ nome, cpf, email }).subscribe({
        next: () => { this.sucesso = 'Cliente cadastrado!'; this.cancelar(); this.carregar(); },
        error: () => { this.erro = 'Erro ao cadastrar cliente. Verifique o CPF.'; }
      });
    }
  }

  cancelar(): void {
    this.exibirForm = false;
    this.editandoId = null;
    this.form.reset();
    setTimeout(() => { this.sucesso = ''; this.erro = ''; }, 3000);
  }

  remover(id: number): void {
    if (!confirm('Remover este cliente?')) return;
    this.service.remover(id).subscribe({
      next: () => { this.sucesso = 'Cliente removido!'; this.carregar(); },
      error: () => { this.erro = 'Erro ao remover. Verifique se há pedidos vinculados.'; }
    });
  }
}
