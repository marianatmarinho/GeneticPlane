

print("\nCarregando modulos de perfil2STL...")

try:
    import numpy as np
    import matplotlib.pyplot as plt
    import os
    print("Modulos de 'perfil2STL' foram carregados com sucesso!")
except ImportError:
    print("ERRO ao importar para 'perfil2STL'\n")
    raise


def ler_perfil(nome):
    try:
        f = open(nome, 'r')
    except IOError:
        print("Erro ao ler arquivo")
        raise

    flines = f.readlines()

    coords = []

    for i in range(1, len(flines)):
        values = flines[i].split()

        try:
            x = float(values[0])
            y = float(values[1])
            coords.append(np.matrix([[x], [y]]))

        except:
            print("Nao foi possivel ler a linha %i" % (i + 1))

    return coords, flines[0]

def ler_escalar_fuselagem(nome, dimx, dimy):
    try:
        f = open(nome, 'r')
    except IOError:
        print("Erro ao ler arquivo")
        raise

    flines = f.readlines()

    coords = []

    for i in range(1, len(flines)):
        values = flines[i].split()

        try:
            x = float(values[0])*dimx
            y = float(values[1])*dimy
            coords.append(np.matrix([[x], [y]]))

        except:
            print("Nao foi possivel ler a linha %i" % (i + 1))

    return coords

def escala_ponta(c_ponta, perfil):
    dim = c_ponta

    pos0 = perfil
    pos1 = []

    for i in range(len(pos0)):
        a = dim * pos0[i]
        pos1.append(a)

    return pos1

def escala(corda, perfil):
    dim = corda
    
    pos0 = perfil
    pos1 = []

    for i in range(len(pos0)):
        a = dim*pos0[i] 
        pos1.append(a)
        
    return pos1


# Translada o perfil para que ele gire em relação ao centro de momento (25% da corda)    
def translacao(x, y, perfil):
    trans2d = np.matrix([[x],[y]])

    pos0 = perfil
    pos1 = []

    for i in range(len(pos0)):
        a = trans2d + pos0[i] 
        pos1.append(a)
    
    return pos1
    
def rotacao2d(alpha, perfil):
    angulo = alpha*np.pi/180
    giro2d = np.matrix([[np.cos(angulo), np.sin(angulo)],[-np.sin(angulo), np.cos(angulo)]])
    
    pos0 = perfil
    pos1 = []

    for i in range(len(pos0)):
        a = giro2d * pos0[i]
        pos1.append(a)
        
    return pos1   
    
def grafico2d(perfil, corda, alpha, titulo): 
    x = np.zeros(len(perfil))
    y = np.zeros(len(perfil))
    
    for i in range(len(perfil)):
        x[i] = perfil[i][0]
        y[i] = perfil[i][1]
        
    plt.figure()
    plt.axis('equal')
    plt.grid('on')
    plt.title(titulo[0:-1] + ": %.1f graus" %(alpha))
    plt.plot(x, y, '.-r')
    plt.xlim([min(x)-0.1*corda, max(x)+0.1*corda])  
    plt.savefig("perfil%s_%.1fGraus.png" %(titulo[0:-1],alpha), bbox_inches='tight', dpi=200)

def objeto3d_xzy(perfil, perfil2, w, w2, arquivo):
    x = np.zeros(len(perfil))
    z = np.zeros(len(perfil))

    for i in range(len(perfil)):
        x[i] = perfil[i][0]
        z[i] = perfil[i][1]

    N = len(x)
    Ni = N // 2

    ## Triangulacao do perfil
    # Vale a regra da mão direita
    tri = np.array([[0, N - 2, 1]])

    for i in range(1, Ni - 1):
        tri = np.append(tri, [[i, N - i - 2, i + 1]], axis=0)

    if z[0] != z[-1]:
        tri = np.append(tri, [[0, N - 1, N - 2]], axis=0)

    if Ni == N / 2.0:  # Caso com número par de pontos
        for i in range(1, Ni - 1):
            tri = np.append(tri, [[i, N - i - 1, N - i - 2]], axis=0)

    else:  # Caso com número ímpar de pontos
        for i in range(1, Ni):
            tri = np.append(tri, [[i, N - i - 1, N - i - 2]], axis=0)

    x2 = np.zeros(len(perfil2))
    z2 = np.zeros(len(perfil2))

    for i in range(len(perfil2)):
        x2[i] = perfil2[i][0]
        z2[i] = perfil2[i][1]

    # Torna 3D; gera um segundo perfil
    x = np.append(x, x2)
    z = np.append(z, z2)
    # y = np.append((envergadura/2)*np.ones(N), -(envergadura/2)*np.ones(N))4
    #y = np.append(0 * np.ones(N), -(envergadura / 2) * np.ones(N))
    y = np.append(w * np.ones(N), w2 * np.ones(N))

    # Triangulação do segundo perfil formado
    # É importante lembrar que agora vale a regra da mão esquerda
    tri = np.append(tri, [[N, 2 * N - 2, 1 + N]], axis=0)

    for i in range(N + 1, N + Ni - 1):
        tri = np.append(tri, [[i, i + 1, N - i - 2]], axis=0)

    if z[0] != z[-1]:
        tri = np.append(tri, [[N, 2 * N - 2, 2 * N - 1]], axis=0)

    if Ni == N / 2.0:  # Caso com número par de pontos
        for i in range(N + 1, N + Ni - 1):
            tri = np.append(tri, [[i, N - i - 2, N - i - 1]], axis=0)

    else:  # Caso com número ímpar de pontos
        for i in range(N + 1, N + Ni):
            tri = np.append(tri, [[i, N - i - 2, N - i - 1]], axis=0)

            # Triangulação da superfície da casca da asa
    # Vale a regra da mão direita
    if z[0] != z[-1]:
        tri = np.append(tri, [[0, N, N - 1], [N, -1, N - 1]], axis=0)

    for i in range(N - 1):
        tri = np.append(tri, [[i, i + 1, N + i]], axis=0)
        tri = np.append(tri, [[i + 1, N + i + 1, N + i]], axis=0)

    for i in range(len(tri)):
        # Calculando o vetor normal
        x0 = x[tri[i][0]]
        x1 = x[tri[i][1]]
        x2 = x[tri[i][2]]

        y0 = y[tri[i][0]]
        y1 = y[tri[i][1]]
        y2 = y[tri[i][2]]

        z0 = z[tri[i][0]]
        z1 = z[tri[i][1]]
        z2 = z[tri[i][2]]

        AB = np.array([x1 - x0, y1 - y0, z1 - z0])
        AC = np.array([x2 - x0, y2 - y0, z2 - z0])
        n = np.cross(AB, AC) / np.linalg.norm(np.cross(AB, AC))

        # Debug: verifica se há faces degeneradas
        if np.isnan(np.linalg.norm(n)) == True:
            print("")
            print(i)
            print('A =', x0, y0, z0)
            print('B =', x1, y1, z1)
            print('C =', x2, y2, z2)
            print('AB =', AB)
            print('AC =', AC)
            print('n =', n)

        # Escrevendo faces
        arquivo.write('  facet normal %e %e %e\n' % (n[0], n[1], n[2]))
        arquivo.write('    outer loop\n')
        arquivo.write('      vertex %e %e %e\n' % (x0, y0, z0))
        arquivo.write('      vertex %e %e %e\n' % (x1, y1, z1))
        arquivo.write('      vertex %e %e %e\n' % (x2, y2, z2))
        arquivo.write('    endloop\n')
        arquivo.write('  endfacet\n')

def objeto3d_xyz(perfil, perfil2, w, w2, arquivo):
    x = np.zeros(len(perfil))
    y = np.zeros(len(perfil))

    for i in range(len(perfil)):
        x[i] = perfil[i][0]
        y[i] = perfil[i][1]

    N = len(x)
    Ni = N // 2

    ## Triangulacao do perfil
    # Vale a regra da mão direita
    tri = np.array([[0, N - 2, 1]])

    for i in range(1, Ni - 1):
        tri = np.append(tri, [[i, N - i - 2, i + 1]], axis=0)

    if y[0] != y[-1]:
        tri = np.append(tri, [[0, N - 1, N - 2]], axis=0)

    if Ni == N / 2.0:  # Caso com número par de pontos
        for i in range(1, Ni - 1):
            tri = np.append(tri, [[i, N - i - 1, N - i - 2]], axis=0)

    else:  # Caso com número ímpar de pontos
        for i in range(1, Ni):
            tri = np.append(tri, [[i, N - i - 1, N - i - 2]], axis=0)

    x2 = np.zeros(len(perfil2))
    y2 = np.zeros(len(perfil2))

    for i in range(len(perfil2)):
        x2[i] = perfil2[i][0]
        y2[i] = perfil2[i][1]
    # Torna 3D; gera um segundo perfil
    x = np.append(x, x2)
    y = np.append(y, y2)
    z = np.append(w * np.ones(N), w2 * np.ones(N))

    # Triangulação do segundo perfil formado
    # É importante lembrar que agora vale a regra da mão esquerda
    tri = np.append(tri, [[N, 2 * N - 2, 1 + N]], axis=0)

    for i in range(N + 1, N + Ni - 1):
        tri = np.append(tri, [[i, i + 1, N - i - 2]], axis=0)

    if y[0] != y[-1]:
        tri = np.append(tri, [[N, 2 * N - 2, 2 * N - 1]], axis=0)

    if Ni == N / 2.0:  # Caso com número par de pontos
        for i in range(N + 1, N + Ni - 1):
            tri = np.append(tri, [[i, N - i - 2, N - i - 1]], axis=0)

    else:  # Caso com número ímpar de pontos
        for i in range(N + 1, N + Ni):
            tri = np.append(tri, [[i, N - i - 2, N - i - 1]], axis=0)

            # Triangulação da superfície da casca da asa
    # Vale a regra da mão direita
    if y[0] != y[-1]:
        tri = np.append(tri, [[0, N, N - 1], [N, -1, N - 1]], axis=0)

    for i in range(N - 1):
        tri = np.append(tri, [[i, i + 1, N + i]], axis=0)
        tri = np.append(tri, [[i + 1, N + i + 1, N + i]], axis=0)

    for i in range(len(tri)):
        # Calculando o vetor normal
        x0 = x[tri[i][0]]
        x1 = x[tri[i][1]]
        x2 = x[tri[i][2]]

        y0 = y[tri[i][0]]
        y1 = y[tri[i][1]]
        y2 = y[tri[i][2]]

        z0 = z[tri[i][0]]
        z1 = z[tri[i][1]]
        z2 = z[tri[i][2]]

        AB = np.array([x1 - x0, z1 - z0, y1 - y0])
        AC = np.array([x2 - x0, z2 - z0, y2 - y0])
        n = np.cross(AB, AC) / np.linalg.norm(np.cross(AB, AC))

        # Debug: verifica se há faces degeneradas
        if np.isnan(np.linalg.norm(n)) == True:
            print("")
            print(i)
            print('A =', x0, y0, y0)
            print('B =', x1, y1, y1)
            print('C =', x2, y2, y2)
            print('AB =', AB)
            print('AC =', AC)
            print('n =', n)

        # Escrevendo faces
        arquivo.write('  facet normal %e %e %e\n' % (n[0], n[1], n[2]))
        arquivo.write('    outer loop\n')
        arquivo.write('      vertex %e %e %e\n' % (x0, y0, z0))
        arquivo.write('      vertex %e %e %e\n' % (x1, y1, z1))
        arquivo.write('      vertex %e %e %e\n' % (x2, y2, z2))
        arquivo.write('    endloop\n')
        arquivo.write('  endfacet\n')

def objeto3d_yzx(perfil, perfil2, w,w2, wz, wz2, arquivo, op):
    y = np.zeros(len(perfil))
    z = np.zeros(len(perfil))

    for i in range(len(perfil)):
        y[i] = perfil[i][0]
        z[i] = perfil[i][1] *wz
        print("\nperfil 1 y:\n", y[i])

    N = len(y)
    Ni = N // 2

    ## Triangulacao do perfil
    # Vale a regra da mão direita
    tri = np.array([[0, N - 2, 1]])

    for i in range(1, Ni - 1):
        tri = np.append(tri, [[i, N - i - 2, i + 1]], axis=0)

    if z[0] != z[-1]:
        tri = np.append(tri, [[0, N - 1, N - 2]], axis=0)

    if Ni == N / 2.0:  # Caso com número par de pontos
        for i in range(1, Ni - 1):
            tri = np.append(tri, [[i, N - i - 1, N - i - 2]], axis=0)

    else:  # Caso com número ímpar de pontos
        for i in range(1, Ni):
            tri = np.append(tri, [[i, N - i - 1, N - i - 2]], axis=0)

    # Torna 3D; gera um segundo perfil
    y2 = np.zeros(len(perfil2))
    z2 = np.zeros(len(perfil2))

    for i in range(len(perfil2)):
        y2[i] = perfil2[i][0]
        if (op==1):
            z2[i] = perfil2[i][1] * wz2
        else:
            z2[i] = perfil2[i][1] + wz2

    # Torna 3D; gera um segundo perfil
    y = np.append(y, y2)
    z = np.append(z, z2)
    # x = np.append((corda / 2) * np.ones(N), -(corda / 2) * np.ones(N))
    x = np.append(w * np.ones(N), w2* np.ones(N))

    # Triangulação do segundo perfil formado
    # É importante lembrar que agora vale a regra da mão esquerda
    tri = np.append(tri, [[N, 2 * N - 2, 1 + N]], axis=0)

    for i in range(N + 1, N + Ni - 1):
        tri = np.append(tri, [[i, i + 1, N - i - 2]], axis=0)

    if z[0] != z[-1]:
        tri = np.append(tri, [[N, 2 * N - 2, 2 * N - 1]], axis=0)

    if Ni == N / 2.0:  # Caso com número par de pontos
        for i in range(N + 1, N + Ni - 1):
            tri = np.append(tri, [[i, N - i - 2, N - i - 1]], axis=0)

    else:  # Caso com número ímpar de pontos
        for i in range(N + 1, N + Ni):
            tri = np.append(tri, [[i, N - i - 2, N - i - 1]], axis=0)

            # Triangulação da superfície da casca da asa
    # Vale a regra da mão direita
    if z[0] != z[-1]:
        tri = np.append(tri, [[0, N, N - 1], [N, -1, N - 1]], axis=0)

    for i in range(N - 1):
        tri = np.append(tri, [[i, i + 1, N + i]], axis=0)
        tri = np.append(tri, [[i + 1, N + i + 1, N + i]], axis=0)

    for i in range(len(tri)):
        # Calculando o vetor normal
        x0 = x[tri[i][0]]
        x1 = x[tri[i][1]]
        x2 = x[tri[i][2]]

        y0 = y[tri[i][0]]
        y1 = y[tri[i][1]]
        y2 = y[tri[i][2]]

        z0 = z[tri[i][0]]
        z1 = z[tri[i][1]]
        z2 = z[tri[i][2]]

        AB = np.array([x1 - x0, y1 - y0, z1 - z0])
        AC = np.array([x2 - x0, y2 - y0, z2 - z0])
        n = np.cross(AB, AC) / np.linalg.norm(np.cross(AB, AC))

        # Debug: verifica se há faces degeneradas
        if np.isnan(np.linalg.norm(n)) == True:
            print("")
            print(i)
            print('A =', x0, y0, z0)
            print('B =', x1, y1, z1)
            print('C =', x2, y2, z2)
            print('AB =', AB)
            print('AC =', AC)
            print('n =', n)

        # Escrevendo faces
        arquivo.write('  facet normal %e %e %e\n' % (n[0], n[1], n[2]))
        arquivo.write('    outer loop\n')
        arquivo.write('      vertex %e %e %e\n' % (x0, y0, z0))
        arquivo.write('      vertex %e %e %e\n' % (x1, y1, z1))
        arquivo.write('      vertex %e %e %e\n' % (x2, y2, z2))
        arquivo.write('    endloop\n')
        arquivo.write('  endfacet\n')


def asa3d_stl(envergadura, perfil, perfil2, nome, perfil_profundor, perfil_profundor2, envergadura_profundor, perfil_leme, perfil_leme2, perfil_leme3, perfil_leme4, envergadura_leme, altura_asa_profundor, fuselagem, corda, cauda, cauda2,distancia_asa_profundor,  altura_fuselagem, nariz, nariz2, asa,xw, leme):

    # Cria o arquivo .stl
    arquivo = open(str(nome[0:-1]) + ' CFD.stl', 'w')
    arquivo.write('solid %s\n' % (str(nome[0:-1])))

    #ASA TRAPEZOIDAL
    if (asa==0):
        #ASA DIREITA
        objeto3d_xzy(perfil, perfil2, 0, (-envergadura/2), arquivo)

        #ASA ESQUERDA
        objeto3d_xzy(perfil2, perfil, (envergadura / 2), 0, arquivo)
    #ASA RETANGULAR
    elif(asa==1):
        # ASA DIREITA
        objeto3d_xzy(perfil, perfil, (envergadura/2), (-envergadura / 2), arquivo)

        # ASA ESQUERDA
        objeto3d_xzy(perfil2, perfil, (envergadura / 2), 0, arquivo)
    #ASA MISTA
    elif(asa==2):
        # ASA RENTANGULAR
        objeto3d_xzy(perfil, perfil, xw*(envergadura / 2), -xw*(envergadura / 2), arquivo)

        # ASA TRAPEZOIDAL DIREITA
        objeto3d_xzy(perfil, perfil2,  -xw*(envergadura / 2), -(envergadura / 2), arquivo)

        # ASA TRAPEZOIDAL ESQUERDA
        objeto3d_xzy(perfil2, perfil, (envergadura / 2),  xw*(envergadura / 2), arquivo)

    #PROFUNDOR DIREITO
    objeto3d_xzy(perfil_profundor, perfil_profundor2, 0, (envergadura_profundor / 2), arquivo)

    #PROFUNDOR ESQUERDO
    objeto3d_xzy(perfil_profundor, perfil_profundor2, 0, (-envergadura_profundor / 2), arquivo)

    #LEME
    #Convencional
    if(leme==1):
        objeto3d_xyz(perfil_leme, perfil_leme2, altura_asa_profundor, altura_asa_profundor+envergadura_leme, arquivo)
    #Em H
    elif(leme==2):
        objeto3d_xyz(perfil_leme, perfil_leme2, altura_asa_profundor-(envergadura_leme/2), altura_asa_profundor + (envergadura_leme/2), arquivo)
        objeto3d_xyz(perfil_leme3, perfil_leme4, altura_asa_profundor-(envergadura_leme/2), altura_asa_profundor + (envergadura_leme/2), arquivo)
    #Em U
    elif (leme == 3):
        objeto3d_xyz(perfil_leme, perfil_leme2, altura_asa_profundor, altura_asa_profundor + envergadura_leme, arquivo)
        objeto3d_xyz(perfil_leme3, perfil_leme4, altura_asa_profundor, altura_asa_profundor + envergadura_leme, arquivo)




    #FUSELAGEM
    objeto3d_yzx(fuselagem,fuselagem, (-0.25 * corda), (0.75 * corda), -1, -1, arquivo,1)

    # CAUDA
    objeto3d_yzx(cauda, cauda2,  (0.75 * corda),distancia_asa_profundor, -1, altura_asa_profundor-altura_fuselagem, arquivo, 0)

    # NARIZ
    objeto3d_yzx(nariz, nariz2, -0.2, 0, 0,-1, arquivo, 1)

    arquivo.write('endsolid %s\n' % (str(nome[0:-1])))
    arquivo.close()

def principal():
    nome = "cr001sm.dat" #raw_input("Nome do arquivo: ")
    try:
        arq = open("variaveis.dat", 'r')
    except IOError:
        print("Erro ao ler arquivo")
        raise

    f=arq.readline()
    asa=float(f)
    f = arq.readline()
    leme = float(f)
    f = arq.readline()
    corda = float(f)
    f = arq.readline()
    envergadura = float(f)
    f = arq.readline()
    alpha = float(f)
    f = arq.readline()
    corda_ponta = float(f)
    f = arq.readline()
    envergadura_profundor = float(f)
    f = arq.readline()
    corda_profundor_raiz = float(f)
    f = arq.readline()
    corda_profundor_ponta = float(f)
    f = arq.readline()
    alpha_profundor = float(f)
    f = arq.readline()
    distancia_asa_profundor = float(f)

    f = arq.readline()
    altura_asa_profundor = float(f)
    f = arq.readline()
    corda_leme_raiz = float(f)
    f = arq.readline()
    corda_leme_ponta = float(f)
    f = arq.readline()
    envergadura_leme = float(f)
    f = arq.readline()
    largura_fuselagem = float(f)
    f = arq.readline()
    altura_fuselagem = float(f)
    f = arq.readline()
    xw = float(f)

    #corda = 0.49 #input("Corda do perfil: ")
    #envergadura = 2.0 #input("envergadura da asa: ")
    #alpha = 2.0 #input("Angulo de ataque em graus: ")
    #corda_ponta = 0.30
    #envergadura_profundor = 1.0
    #corda_profundor_raiz = 0.30
    #corda_profundor_ponta = 0.15
    #alpha_profundor = 5.0
    #distancia_asa_profundor = 2.0 #diatncia do ca da asa ate o CA do profundor
    #altura_asa_profundor = 0.5 #altura do CA da Asa até o CA do profundor
    #corda_leme_raiz = 0.30
    #corda_leme_ponta = 0.15
    #envergadura_leme=0.4
    #largura_fuselagem=0.1
    #altura_fuselagem=0.15
    #xw = 0.30
    #asa=1
    #leme=3

    if(asa==0):
        perfil = rotacao2d(alpha, translacao(-0.25*corda, 0, escala(corda, ler_perfil("cr001sm.dat")[0])))
        perfil2 = rotacao2d(alpha,translacao(-0.25 * corda_ponta, 0, escala_ponta(corda_ponta, ler_perfil("cr001sm.dat")[0])))
    elif(asa==1):
        perfil = rotacao2d(alpha, translacao(-0.25 * corda, 0, escala(corda, ler_perfil("cr001sm.dat")[0])))
        perfil2 = rotacao2d(alpha, translacao(-0.25 * corda, 0, escala(corda, ler_perfil("cr001sm.dat")[0])))
    elif(asa==2):
        print("entrei")
        perfil = rotacao2d(alpha, translacao(-0.25 * corda, 0, escala(corda, ler_perfil("cr001sm.dat")[0])))
        perfil2 = rotacao2d(alpha,
                            translacao(-0.25 * corda_ponta, 0, escala_ponta(corda_ponta, ler_perfil("cr001sm.dat")[0])))

    perfil_profundor = translacao(distancia_asa_profundor, altura_asa_profundor, (rotacao2d(alpha_profundor, translacao( (-0.25 * corda_profundor_raiz),
                                                             0, escala(corda_profundor_raiz,
                                                                                          ler_perfil("cr001sm.dat")[
                                                                                              0])))))
    perfil_profundor2 = translacao(distancia_asa_profundor,altura_asa_profundor, (rotacao2d(alpha_profundor, translacao( (-0.25 * corda_profundor_ponta),
                                                              0, escala(corda_profundor_ponta,
                                                                                           ler_perfil("cr001sm.dat")[
                                                                                               0])))))
    # Convencional
    if (leme == 1):
        perfil_leme = translacao(distancia_asa_profundor + (-0.25 * corda_leme_raiz), 0,
                                      escala(corda_leme_raiz,
                                             ler_perfil("cr001sm.dat")[0]))
        perfil_leme2 = translacao(distancia_asa_profundor + (-0.25 * corda_leme_ponta),
                                       0, escala(corda_leme_ponta,
                                                                    ler_perfil("cr001sm.dat")[0]))
        perfil_leme3 = translacao(distancia_asa_profundor + (-0.25 * corda_leme_raiz), 0,
                                 escala(corda_leme_raiz,
                                        ler_perfil("cr001sm.dat")[0]))
        perfil_leme4 = translacao(distancia_asa_profundor + (-0.25 * corda_leme_ponta),
                                  0, escala(corda_leme_ponta,
                                            ler_perfil("cr001sm.dat")[0]))
    #Em H
    elif (leme == 2):
        perfil_leme = translacao(distancia_asa_profundor + (-0.25 * corda_profundor_ponta), (envergadura_profundor / 2),
                                 escala(corda_leme_raiz,
                                        ler_perfil("cr001sm.dat")[0]))
        perfil_leme2 = translacao(distancia_asa_profundor + (-0.25 * corda_profundor_ponta),
                                  (envergadura_profundor / 2), escala(corda_leme_ponta,
                                                                      ler_perfil("cr001sm.dat")[0]))
        perfil_leme3 = translacao(distancia_asa_profundor + (-0.25 * corda_profundor_ponta),
                                  (-envergadura_profundor / 2),
                                  escala(corda_leme_raiz,
                                         ler_perfil("cr001sm.dat")[0]))
        perfil_leme4 = translacao(distancia_asa_profundor + (-0.25 * corda_profundor_ponta),
                                  (-envergadura_profundor / 2), escala(corda_leme_ponta,
                                                                       ler_perfil("cr001sm.dat")[0]))
    #Em U
    elif(leme==3):
        perfil_leme = translacao(distancia_asa_profundor + (-0.25*corda_profundor_ponta), (envergadura_profundor / 2),
                                 escala(corda_leme_raiz,
                                        ler_perfil("cr001sm.dat")[0]))
        perfil_leme2 = translacao(distancia_asa_profundor + (-0.25* corda_profundor_ponta),
                                  (envergadura_profundor / 2), escala(corda_leme_ponta,
                                            ler_perfil("cr001sm.dat")[0]))
        perfil_leme3 = translacao(distancia_asa_profundor + (-0.25 * corda_profundor_ponta), (-envergadura_profundor / 2),
                                 escala(corda_leme_raiz,
                                        ler_perfil("cr001sm.dat")[0]))
        perfil_leme4 = translacao(distancia_asa_profundor + (-0.25 * corda_profundor_ponta),
                                  (-envergadura_profundor / 2), escala(corda_leme_ponta,
                                                                      ler_perfil("cr001sm.dat")[0]))
    fuselagem =  ler_escalar_fuselagem("fuselagem.dat", largura_fuselagem,
                                                                                    altura_fuselagem)
    cauda = ler_escalar_fuselagem("fuselagem.dat", largura_fuselagem,
                                      altura_fuselagem)
    cauda2 = ler_escalar_fuselagem("fuselagem.dat", largura_fuselagem,
                                  altura_fuselagem)

    nariz = ler_escalar_fuselagem("fuselagem.dat", largura_fuselagem,
                                   altura_fuselagem)

    nariz2 = ler_escalar_fuselagem("fuselagem.dat", largura_fuselagem,
                                  altura_fuselagem)
    print("perfil")
    asa3d_stl(envergadura, perfil, perfil2, ler_perfil("cr001sm.dat")[1], perfil_profundor, perfil_profundor2, envergadura_profundor, perfil_leme, perfil_leme2, perfil_leme3, perfil_leme4, envergadura_leme, altura_asa_profundor, fuselagem, corda, cauda, cauda2, distancia_asa_profundor,  altura_fuselagem, nariz, nariz2, asa, xw, leme)
    print("asa3d")
    #grafico2d(perfil, corda, alpha, ler_perfil("cr001sm.dat")[1])
    
    print ("\nO arquivo '.stl' foi gerado com sucesso.\n")

if __name__ == "__main__":
    principal()
