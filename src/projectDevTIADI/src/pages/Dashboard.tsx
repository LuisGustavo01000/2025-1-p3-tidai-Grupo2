import React, { useState, useEffect } from 'react';
import api from '../services/api';

interface Transacao {
  id: number;
  descricao: string;
  valor: number;
  tipo: string;
  data: string;
}

interface Usuario {
  id: number;
  nome: string;
  email: string;
  transacoes: Transacao[];
  endividado: boolean;
}

const Dashboard: React.FC = () => {
  const [usuario, setUsuario] = useState<Usuario | null>(null);
  const [saldoTotal, setSaldoTotal] = useState(0);

  useEffect(() => {
    loadUsuario();
  }, []);

  const loadUsuario = async () => {
    try {
      const response = await api.get('/pessoa/1'); // Assuming user ID 1 for example
      setUsuario(response.data);
      calcularSaldo(response.data.transacoes);
    } catch (error) {
      console.error('Erro ao carregar usuário:', error);
    }
  };

  const calcularSaldo = (transacoes: Transacao[]) => {
    const saldo = transacoes.reduce((acc, transacao) => {
      return transacao.tipo === 'Receita' 
        ? acc + transacao.valor 
        : acc - transacao.valor;
    }, 0);
    setSaldoTotal(saldo);
  };

  return (
    <div className="bg-white p-6 rounded-lg shadow-lg">
      {usuario && (
        <>
          <h1 className="text-2xl font-bold mb-6">Dashboard de {usuario.nome}</h1>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
            <div className="bg-blue-50 p-4 rounded-lg">
              <h2 className="text-lg font-semibold mb-2">Saldo Total</h2>
              <p className={`text-2xl font-bold ${saldoTotal >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                R$ {saldoTotal.toFixed(2)}
              </p>
            </div>
            
            <div className="bg-blue-50 p-4 rounded-lg">
              <h2 className="text-lg font-semibold mb-2">Status</h2>
              <p className={`text-lg ${usuario.endividado ? 'text-red-600' : 'text-green-600'}`}>
                {usuario.endividado ? 'Endividado' : 'Saudável'}
              </p>
            </div>
          </div>

          <div className="mt-8">
            <h2 className="text-xl font-semibold mb-4">Últimas Transações</h2>
            <div className="overflow-x-auto">
              <table className="min-w-full bg-white">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Data</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Descrição</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Tipo</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Valor</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-200">
                  {usuario.transacoes.map((transacao) => (
                    <tr key={transacao.id}>
                      <td className="px-6 py-4 whitespace-nowrap">
                        {new Date(transacao.data).toLocaleDateString()}
                      </td>
                      <td className="px-6 py-4">{transacao.descricao}</td>
                      <td className="px-6 py-4">{transacao.tipo}</td>
                      <td className={`px-6 py-4 ${transacao.tipo === 'Receita' ? 'text-green-600' : 'text-red-600'}`}>
                        R$ {transacao.valor.toFixed(2)}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </>
      )}
    </div>
  );
};

export default Dashboard;