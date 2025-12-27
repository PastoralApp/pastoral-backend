using AutoMapper;
using PA.Application.DTOs;
using PA.Application.Interfaces.Services;
using PA.Domain.Entities;
using PA.Domain.Enums;
using PA.Domain.Interfaces;

namespace PA.Application.Services;

public class MedalhaService : IMedalhaService
{
    private readonly IMedalhaRepository _medalhaRepository;
    private readonly IMapper _mapper;

    public MedalhaService(IMedalhaRepository medalhaRepository, IMapper mapper)
    {
        _medalhaRepository = medalhaRepository;
        _mapper = mapper;
    }

    public async Task<MedalhaDto?> GetByIdAsync(Guid id)
    {
        var medalha = await _medalhaRepository.GetByIdAsync(id);
        return medalha == null ? null : _mapper.Map<MedalhaDto>(medalha);
    }

    public async Task<IEnumerable<MedalhaDto>> GetAllAsync()
    {
        var medalhas = await _medalhaRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<MedalhaDto>>(medalhas);
    }

    public async Task<IEnumerable<MedalhaDto>> GetByJogoIdAsync(Guid jogoId)
    {
        var medalhas = await _medalhaRepository.GetByJogoIdAsync(jogoId);
        return _mapper.Map<IEnumerable<MedalhaDto>>(medalhas);
    }

    public async Task<IEnumerable<MedalhaDto>> GetByGrupoIdAsync(Guid grupoId)
    {
        var medalhas = await _medalhaRepository.GetByGrupoIdAsync(grupoId);
        return _mapper.Map<IEnumerable<MedalhaDto>>(medalhas);
    }

    public async Task<IEnumerable<MedalhaDto>> GetByParticipanteIdAsync(Guid participanteId)
    {
        var medalhas = await _medalhaRepository.GetByParticipanteIdAsync(participanteId);
        return _mapper.Map<IEnumerable<MedalhaDto>>(medalhas);
    }

    public async Task<IEnumerable<MedalhaDto>> GetByAnoAsync(int ano)
    {
        var medalhas = await _medalhaRepository.GetByAnoAsync(ano);
        return _mapper.Map<IEnumerable<MedalhaDto>>(medalhas);
    }

    public async Task<IEnumerable<MedalhaDto>> GetByTipoAsync(TipoMedalha tipo)
    {
        var medalhas = await _medalhaRepository.GetAllAsync();
        var filtered = medalhas.Where(m => m.Tipo == tipo);
        return _mapper.Map<IEnumerable<MedalhaDto>>(filtered);
    }

    public async Task<MedalhaDto> CreateAsync(CreateMedalhaDto dto)
    {
        var medalha = new Medalha
        {
            Tipo = dto.Tipo,
            JogoId = dto.JogoId,
            ModalidadeId = dto.ModalidadeId,
            ProvaId = dto.ProvaId,
            GrupoId = dto.GrupoId,
            Ano = dto.Ano,
            DataConquista = dto.DataConquista,
            Descricao = dto.Descricao,
            Observacoes = dto.Observacoes
        };

        var created = await _medalhaRepository.AddAsync(medalha);

        if (dto.ParticipantesIds.Any())
        {
            var participantes = dto.ParticipantesIds.Select(userId => new MedalhaParticipante
            {
                MedalhaId = created.Id,
                UserId = userId,
                DataRecebimento = dto.DataConquista
            }).ToList();

            foreach (var participante in participantes)
            {
                created.Participantes.Add(participante);
            }

            await _medalhaRepository.UpdateAsync(created);
        }

        return _mapper.Map<MedalhaDto>(created);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _medalhaRepository.DeleteAsync(id);
    }
}
