///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabalhoPratico;
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class HeightMap {
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Variables
    public Matrix worldMatrix;
    public int mHeight, mWidth, vertexCount, indexCount;
    public VertexPositionNormalTexture[] vertices;
    private short[] indices;
    private BasicEffect effect;
    private Color[] terreno;
    private Texture2D map, terrain;
    private float scale;
    private VertexBuffer vertexBuffer;
    private IndexBuffer indexBuffer;
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Constructor
    public HeightMap(GraphicsDevice device, ContentManager content, Camera camera)
    {
        worldMatrix = Matrix.Identity;
        effect = new BasicEffect(device);
        // Calcula a aspectRatio, a view matrix e a projeção
        float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
        effect.View = Matrix.CreateLookAt(new Vector3(20.0f, 10.0f, 20.0f), new Vector3(80, 0, 0), Vector3.Up);
        effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 100.0f);
        effect.TextureEnabled = true;
        //< LIGHTING >
        effect.DirectionalLight0.Enabled = true;
        effect.DirectionalLight0.DiffuseColor  = new Vector3(0.0f, 0.0f ,0.0f); // COR DO RAIO DE LUZ
        effect.DirectionalLight0.Direction     = new Vector3(0.0f,-20.0f,0.0f); // DIREÇÂO DO RAIO DE LUZ
        effect.DirectionalLight0.SpecularColor = new Vector3(1.0f, 1.0f ,1.0f); // CRIA BRILHO
        //< MAPA >
        map = content.Load<Texture2D>("HeightMap"); //Mapa para Calculo
        terrain = content.Load<Texture2D>("sand");  //Mapa para Cosmetics
        scale = 0.05f;
        mHeight = map.Height;
        mWidth  = map.Width;
        terreno = new Color[mHeight * mWidth]; //Array de cores, para guardar a cor de cada vertice

        CreateVertexAndIndex(device);
        effect.EnableDefaultLighting();
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Normais Do Mapa
    private void CreateNormais(VertexPositionNormalTexture[] vertices) {
        for (int x = 0; x < mWidth; x++) {
            for (int z = 0; z < mHeight; z++) {
                #region LADO ESQUERDO
                if (x == 0 && z != 0 && z != mHeight - 1)
                {
                    Vector3 normal = Vector3.Zero;
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z + 1) * mWidth + (x + 0)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z + 1) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z + 1) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z + 0) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z - 0) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z - 1) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z - 1) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z - 1) * mWidth + (x + 0)].Position - vertices[z * mWidth + x].Position));
                    normal /= 4;
                    normal = Vector3.Normalize(normal);
                    vertices[z * mWidth + x].Normal = normal;
                }
                #endregion
                #region LADO DIREITO
                else if (x == mWidth - 1 && z != 0 && z != mHeight - 1) {
                    Vector3 normal = Vector3.Zero;
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z - 1) * mWidth + (x - 0)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z - 1) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z - 1) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z - 0) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z - 0) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z + 1) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z + 1) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z + 1) * mWidth + (x - 0)].Position - vertices[z * mWidth + x].Position));
                    normal /= 4;
                    normal = Vector3.Normalize(normal);
                    vertices[z * mWidth + x].Normal = normal;
                }
                #endregion
                #region LADO CIMA
                else if (z == 0 && x != 0 && x != mWidth - 1) {
                    Vector3 normal = Vector3.Zero;
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z + 0) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z + 1) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z + 1) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z + 1) * mWidth + (x + 0)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z + 1) * mWidth + (x + 0)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z + 1) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z + 1) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z + 0) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position));
                    normal /= 4;
                    normal = Vector3.Normalize(normal);
                    vertices[z * mWidth + x].Normal = normal;
                }
                #endregion
                #region LADO BAIXO
                else if (z == mHeight - 1 && x != 0 && x != mWidth - 1)
                {
                    Vector3 normal = Vector3.Zero;
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z - 0) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z - 1) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z - 1) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z - 1) * mWidth + (x + 0)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z - 1) * mWidth + (x + 0)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z - 1) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z - 1) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z - 0) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position));
                    normal /= 4;
                    normal = Vector3.Normalize(normal);
                    vertices[z * mWidth + x].Normal = normal;
                }
                #endregion
                #region MEIO
                else if (x != 0 && x != mWidth - 1 && z != 0 && z != mHeight - 1) {
                    Vector3 normal = Vector3.Zero;
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z + 0) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z - 1) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z - 1) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z - 1) * mWidth + (x + 0)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z - 1) * mWidth + (x + 0)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z - 1) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z - 1) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z - 0) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z + 0) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z + 1) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z + 1) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z + 1) * mWidth + (x - 0)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z + 1) * mWidth + (x - 0)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z + 1) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z + 1) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z + 0) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position));
                    normal /= 8;
                    normal = Vector3.Normalize(normal);
                    vertices[z * mWidth + x].Normal = normal;
                }
                #endregion
                #region CANTO SUPERIOR DIREITO
                else if (x == mWidth - 1 && z == 0) {
                    Vector3 normal = Vector3.Zero;
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z + 0) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z + 1) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z + 1) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z + 1) * mWidth + (x - 0)].Position - vertices[z * mWidth + x].Position));
                    normal /= 2;
                    normal = Vector3.Normalize(normal);
                    vertices[z * mWidth + x].Normal = normal;
                }
                #endregion
                #region CANTO SUPERIOR ESQUERDO
                else if (x == 0 && z == 0)
                {
                    Vector3 normal = Vector3.Zero;
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z + 1) * mWidth + (x + 0)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z + 1) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z + 1) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z + 0) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position));
                    normal /= 2;
                    normal = Vector3.Normalize(normal);
                    vertices[z * mWidth + x].Normal = normal;
                }
                #endregion
                #region CANTO INFERIOR DIREITO
                else if (x == mWidth - 1 && z == mHeight - 1)
                {
                    Vector3 normal = Vector3.Zero;
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z - 1) * mWidth + (x - 0)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z - 1) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z - 1) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z - 0) * mWidth + (x - 1)].Position - vertices[z * mWidth + x].Position));
                    normal /= 2;
                    normal = Vector3.Normalize(normal);
                    vertices[z * mWidth + x].Normal = normal;
                }
                #endregion
                #region CANTO INFERIOR ESQUERDO
                else if (x == 0 && z == mHeight - 1) {
                    Vector3 normal = Vector3.Zero;
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z - 0) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z - 1) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position));
                    normal += Vector3.Cross(Vector3.Normalize(vertices[(z - 1) * mWidth + (x + 1)].Position - vertices[z * mWidth + x].Position), Vector3.Normalize(vertices[(z - 1) * mWidth + (x - 0)].Position - vertices[z * mWidth + x].Position));
                    normal /= 2;
                    normal = Vector3.Normalize(normal);
                    vertices[z * mWidth + x].Normal = normal;
                }
                #endregion
            }
        }
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Vertices e Indices
    private void CreateVertexAndIndex(GraphicsDevice device) {
        vertexCount = mWidth * mHeight;                          //NUM VERTICES;
        vertices = new VertexPositionNormalTexture[vertexCount]; //ARRAY DE VERTICES;
        float colorY;                                            //VARIAVEL USADA PARA O VALOR DOS CINZENTOS;
        map.GetData<Color>(terreno);                             //DÁ "SCAN" A COR PRESENTE;
        //< VERTICES >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        for (int x = 0; x < mWidth; x++)
        {
            for (int z = 0; z < mHeight; z++)
            {
                //<COLOR HEIGHT>
                colorY = terreno[z * mWidth + x].R * scale;          //GUARDA A COR CONVERTENDO PARA FLOAT;
                                                                     //<VERTICES>
                vertices[z * mWidth + x] = new VertexPositionNormalTexture(new Vector3(x, colorY, z), Vector3.Up, new Vector2(x % 2, z % 2));
                //GUARDAR VERTICE COM DEVIDA ALTURA DADA PELA VARIAVEL "colorY";
                //NOS INDICES EM QUE x E PARA A COORDENADA 0 E IMPAR É 1;.
            }
        }
        CreateNormais(vertices);
        vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), vertices.Length, BufferUsage.None);
        vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);
        //<INDICES VERTICAL>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        #region Organização dos Indices //EXPLICAÇÃO;
        //1º Strip
        //0
        //1
        //128
        //129
        //256
        //257
        //2ªStrip
        //1
        //2
        //129
        //130
        //257
        //258
        #endregion
        indexCount = 2 * mWidth * (mHeight - 1);                 //NUM INDICES;
        indices = new short[indexCount];                         //TOTAL INDICES;
        for (int y = 0; y < mHeight - 1; y++)
        {
            for (int x = 0; x < mWidth; x++)
            {
                indices[2 * x + 0 + (2 * y * mHeight)] = (short)(x * mHeight + y);
                indices[2 * x + 1 + (2 * y * mHeight)] = (short)(x * mHeight + 1 + y);
            }
        }
        indexBuffer = new IndexBuffer(device, typeof(short), indices.Length, BufferUsage.None);
        indexBuffer.SetData<short>(indices);
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region CalculateInterpolation
    public float CalculateInterpolation(float cameraX, float cameraZ)
    {
        if (cameraX > mWidth || cameraX < 0 || cameraZ > mHeight || cameraZ < 0)
        {
            return 2.0f;  // ALTURA FORA DO MAPA
        }
        else
        {
            VertexPositionNormalTexture vertex1, vertex2, vertex3, vertex4;
            vertex1 = vertices[(int)(cameraZ) * mWidth + (int)(cameraX)];
            vertex2 = vertices[(int)(cameraZ) * mWidth + (int)(cameraX + 1)];
            vertex3 = vertices[(int)(cameraZ + 1) * mWidth + (int)(cameraX)];
            vertex4 = vertices[(int)(cameraZ + 1) * mWidth + (int)(cameraX + 1)];

            float peso1, peso2;
            float inter1, inter2, interFinal;
            //Interpolação 1
            peso1 = cameraX - vertex1.Position.X;
            peso2 = 1 - peso1;

            inter1 = vertex1.Position.Y * peso2 + vertex2.Position.Y * peso1;

            //Interpolação 2
            inter2 = vertex3.Position.Y * peso2 + vertex4.Position.Y * peso1;

            //Interpolação 3
            peso1 = cameraZ - vertex1.Position.Z;
            peso2 = 1 - peso1;

            interFinal = inter1 * peso2 + inter2 * peso1;

            return interFinal;
        }
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Draw
    public void Draw(GraphicsDevice device, Camera camera) {
        int numVertices = mHeight * 2;
        int startIndex = mHeight * 2;
        int numPrimitives = mHeight * 2 - 2;
        effect.World = worldMatrix;
        effect.View = camera.view;
        effect.Projection = camera.projection;
        device.SetVertexBuffer(vertexBuffer);
        device.Indices = indexBuffer;
        effect.Texture = terrain;
        effect.CurrentTechnique.Passes[0].Apply();
        for (int i = 0; i < mWidth; i++)
        {
            device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, numVertices, i * startIndex, numPrimitives);
            //num de vertices vai ser igual a altura do mapa * 2, pois cada strip usa 2 colunas de vertices
            //o start index é igual ao primeiro index de cada coluna, neste caso andando de 256 em 256.
            //o numero de primitivas e igual a altura do mapa * 2, -1 para nao criar um triangulo entre o primeiro e o ultimo indice
        }
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////