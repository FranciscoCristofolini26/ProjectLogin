import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, Validators, NonNullableFormBuilder } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common'; // <— AQUI

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule], // <— AQUI
  templateUrl: './signup.html',
  styleUrls: ['./signup.css'],
})
export class Signup {
  private api = 'http://localhost:5093';
  private fb = inject(NonNullableFormBuilder);
  private http = inject(HttpClient);

  loading = false;
  ok = false;
  erro = '';

  form = this.fb.group({
    email: this.fb.control('', { validators: [Validators.required, Validators.email] }),
    userName: this.fb.control('', { validators: [Validators.required, Validators.minLength(3)] }),
    senha: this.fb.control('', { validators: [Validators.required, Validators.minLength(6)] }),
  });

  onSubmit() {
  console.log('SUBMIT disparado');

  // LOGS DE DIAGNÓSTICO
  console.log('form value:', this.form.getRawValue());
  console.log('form valid?', this.form.valid, 'status:', this.form.status);

  // ⚠️ TEMPORÁRIO: não retorne, mesmo inválido (só pra ver o POST no Network)
  // if (this.form.invalid) return;

  this.loading = true;
  this.ok = false;
  this.erro = '';

  const { email, userName, senha } = this.form.getRawValue();
  const payload = { email, userName, senhaHash: senha };

  console.log('POST para:', `${this.api}/usuarios`, 'payload:', payload);

  this.http.post(`${this.api}/usuarios`, payload).subscribe({
    next: (res) => {
      console.log('POST OK', res);
      this.loading = false;
      this.ok = true;
      this.form.reset();
    },
    error: (e) => {
      console.error('POST ERRO', e);
      this.loading = false;
      this.erro = e?.error?.message ?? 'Erro ao cadastrar.';
    }
  });
}


}
